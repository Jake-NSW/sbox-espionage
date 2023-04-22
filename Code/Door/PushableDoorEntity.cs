using System;
using System.Numerics;
using Editor;
using Sandbox;
using ModelDoorSounds = Sandbox.DoorEntity.ModelDoorSounds;

namespace Woosh.Espionage;

[Library( "esp_door" ), HammerEntity, Category( "Gameplay" ), Icon( "door_front" ), Model]
public sealed partial class PushableDoorEntity : AnimatedEntity, IPushable
{
	public enum DoorStates
	{
		Closed,
		Open,
		FullyOpen,
		Locked,
	}

	[Property] public string Display { get; set; } = "Door";

	// Utility

	[Property, FGDType( "target_source" )] public string Partner { get; set; }
	[Property] public bool Inverted { get; set; }

	// Angle

	[Property] public Vector2 MaxAngle { get; set; } = new Vector2( 90, 0 );
	[Property] public int ClosedAngle { get; set; } = 0;
	[Property] public int DefaultAngle { get; set; } = 0;
	[Property] public bool RandomAngle { get; set; } = false;

	// Physics

	[Property( Title = "Weight (kg)" )] public int Weight { get; set; } = 15;
	[Property] public float Stiffness { get; set; } = 3.5f;

	// Sound

	[Property, Category( "Sounds" ), FGDType( "curve" )]
	public Curve ImpactLoudness { get; set; }

	public PushableDoorEntity()
	{
		Transmit = TransmitType.Pvs;
	}

	public override void Spawn()
	{
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		Initial = Transform;

		EnableAllCollisions = true;
		UsePhysicsCollision = true;

		m_TargetRotation = Initial.Rotation;
	}

	// Physics

	public Transform Initial { get; set; }

	private float m_Force;
	private Rotation m_TargetRotation = Rotation.Identity;

	[Event.Tick.Server]
	private void Tick()
	{
		var maxRot = Initial.Rotation.RotateAroundAxis( Transform.NormalToLocal( Initial.Rotation.Up * (Inverted ? 1 : -1) ), MaxAngle.x );
		var minRot = Initial.Rotation.RotateAroundAxis( Transform.NormalToLocal( Initial.Rotation.Up * (Inverted ? 1 : -1) ), MaxAngle.y );

		var bounds = CollisionBounds;
		DebugOverlay.Box( Position, minRot, bounds.Mins, bounds.Maxs, Color.Orange );
		DebugOverlay.Box( Position, maxRot, bounds.Mins, bounds.Maxs, Color.Blue );

		DebugOverlay.Line( Initial.Position, Initial.Position + Initial.Rotation.Up * 48, Color.Orange );

		var stiffness = MathF.Max( 1, Stiffness );
		var weight = MathF.Max( 1, Weight );

		// Decay Force
		m_Force = !m_Force.AlmostEqual( 0, 0.005f ) ? m_Force.LerpTo( 0, weight / stiffness * Time.Delta ) : 0;

		// m_Rotation = m_LastAngle.LerpTo( m_LastAngle + Force, Stiffness / Weight * Stiffness * 10 * Time.Delta );
		// m_LastAngle = m_LastAngle.Clamp( 0, MaxAngle );

		var axis = Transform.NormalToLocal( Initial.Rotation.Up );
		var potential = m_TargetRotation * Rotation.FromAxis( axis, m_Force );
		potential = potential.Clamp( Initial.Rotation, MaxAngle.x, out var change );

		var dot = Vector3.Dot( Inverted ? potential.Forward : potential.Backward, Initial.Rotation.Left );
		var angle = Rotation.Difference( potential, Initial.Rotation ).Angle();

		if ( dot < 0 )
			angle *= -1;

		// Clamp to Axis
		if ( angle > MaxAngle.y && angle < MaxAngle.x )
			m_TargetRotation = potential;
		// else if ( angle < 0 )
		// m_TargetRotation = Initial.Rotation;

		DebugOverlay.Text( $"Angle - {angle}\nDot - {dot}\nChange - {change}", Position );
		Rotation = m_TargetRotation;

		if ( angle.AlmostEqual( MaxAngle.x, 0.005f ) || angle < MaxAngle.y )
		{
			m_Force = m_Force / 2 * -1;
			// PlayClientSound( DoorStates.FullyOpen, MathF.Abs( m_Force ) / 1.4f );
		}
	}

	// Sounds

	private ModelDoorSounds m_Sounds;

	public override void OnNewModel( Model model )
	{
		base.OnNewModel( model );

		if ( model is { IsError: false } && model.TryGetData( out ModelDoorSounds sounds ) )
		{
			m_Sounds = sounds;
			return;
		}

		m_Sounds = new ModelDoorSounds { FullyOpenSound = "break_pottery" };
	}

	[ClientRpc]
	private void PlayClientSound( DoorStates state, float volume )
	{
		var sound = state switch
		{
			DoorStates.Closed => m_Sounds.CloseSound,
			DoorStates.Open => m_Sounds.OpenSound,
			DoorStates.FullyOpen => m_Sounds.FullyOpenSound,
			DoorStates.Locked => m_Sounds.LockedSound,
			_ => throw new ArgumentOutOfRangeException( nameof(state), state, null )
		};

		Log.Info( $"Playing {sound}, with volume {volume}" );
		Sound.FromWorld( sound, Position ).SetVolume( volume );
	}

	// Push

	public override void TakeDamage( DamageInfo info )
	{
		/*
		base.TakeDamage( info );

		// Don't do anything if its closed
		if ( info.Attacker.Position.Distance( Position ) <= 64 || !IsClosed )
		{
			var force = info.Force.Length / 10;
			Push( info.Position, info.Attacker as Pawn, force );
		}
		*/
	}

	public void Push( Vector3 from, Entity invoker, float force )
	{
		if ( !IsAuthority )
			return;

		var direction = Vector3.Dot( (from - Position).Normal, Rotation.Left );
		force *= (direction > 0 ? 1 : -1) * (Inverted ? 1 : -1);
		m_Force += (force / (MathF.Max( 1, Stiffness * Stiffness ))) * 8;
		LastAttacker = invoker;
	}

	public void Push( Entity entity, float force ) => Push( entity.Position, entity, force );

}
