using System;
using Editor;
using Sandbox;
using Woosh.Data;

// Nice Syntax
using ModelDoorSounds = Sandbox.DoorEntity.ModelDoorSounds;

namespace Woosh.Espionage;

[Library( "esp_hinged_door" ), HammerEntity, Category( "Gameplay" ), Icon( "door_front" ), Model]
public sealed partial class PushableHingedDoorEntity : AnimatedEntity, IPushable, IProfiled
{
	public enum DoorStates : byte
	{
		Closed,
		Open,
		Bounced,
		Locked
	}

	public Profile? Profile { get; }

	// Utility

	[Property, FGDType( "target_source" )] public string Partner { get; set; }
	[Property] public bool Inverted { get; set; }

	// Angle

	[Property] public Vector2 MaxAngle { get; set; } = new Vector2( 90, 0 );
	[Property] public Vector2 SpawnAngle { get; set; } = new Vector2( 0, 0 );
	[Property] public int ClosedAngle { get; set; } = 0;

	// Physics

	[Property] public bool SleepWhenClosed { get; set; } = true;
	[Property( Title = "Weight (kg)" )] public int Weight { get; set; } = 15;
	[Property] public float Stiffness { get; set; } = 3.5f;

	// Profile

	[Property] private string Display { get; set; } = "Door";
	
	public PushableHingedDoorEntity()
	{
		Transmit = TransmitType.Pvs;
		Profile = new Profile( Display ) { Brief = "Push & Pull", Binding = "Scroll" };
	}

	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		Initial = Transform;

		EnableAllCollisions = true;
		UsePhysicsCollision = true;

		// Setup Rotation
		if ( SpawnAngle != default )
			RotateToRelativeAngle( Game.Random.Float( SpawnAngle.x, SpawnAngle.y ) * (Inverted ? 1 : -1) );
	}

	// Debug

	[ConVar.Server( "esp_door_debug" )] private static bool EnabledDoorDebug { get; set; }

	// Physics

	public Transform Initial { get; set; }
	private DoorStates m_State;
	private float m_Force;

	[Event.Tick.Server]
	private void Tick()
	{
		// Debug

		if ( EnabledDoorDebug )
		{
			var maxRot = Initial.Rotation.RotateAroundAxis( Transform.NormalToLocal( Initial.Rotation.Up * (Inverted ? 1 : -1) ), MaxAngle.x );
			var minRot = Initial.Rotation.RotateAroundAxis( Transform.NormalToLocal( Initial.Rotation.Up * (Inverted ? 1 : -1) ), MaxAngle.y );
			var closedRot = Initial.Rotation.RotateAroundAxis( Transform.NormalToLocal( Initial.Rotation.Up * (Inverted ? 1 : -1) ), ClosedAngle );

			var bounds = CollisionBounds;
			DebugOverlay.Box( Position, Initial.Rotation, bounds.Mins, bounds.Maxs, Color.Gray );
			DebugOverlay.Box( Position, minRot, bounds.Mins, bounds.Maxs, Color.Orange );
			DebugOverlay.Box( Position, maxRot, bounds.Mins, bounds.Maxs, Color.Blue );
			DebugOverlay.Box( Position, closedRot, bounds.Mins, bounds.Maxs, Color.Cyan );
		}

		// Values

		var stiffness = MathF.Max( 0.1f, Stiffness );
		var weight = MathF.Max( 0.1f, Weight );

		m_Force = !m_Force.AlmostEqual( 0, 0.005f ) ? m_Force.LerpTo( 0, weight / stiffness * Time.Delta ) : 0;

		RotateToRelativeAngle( m_Force );
	}

	private void RotateToRelativeAngle( float force )
	{
		Game.AssertServer();

		var axis = Transform.NormalToLocal( Initial.Rotation.Up );
		var potential = Rotation.FromAxis( axis, force );
		var dot = Vector3.Dot( Inverted ? (Rotation * potential).Forward : (Rotation * potential).Backward, Initial.Rotation.Left );

		// Angle
		var angle = Rotation.Difference( Rotation * potential, Initial.Rotation ).Angle();
		angle *= dot < 0 ? -1 : 1;

		var target = Rotation * potential;

		// Clamp to Axis
		if ( angle > MaxAngle.y && angle < MaxAngle.x )
			Rotation = target;

		if ( EnabledDoorDebug )
			DebugOverlay.Text( $"Force - {force}\nAngle - {angle}\nDot - {dot}\nRotation - {Rotation.Angles()}\nState - {m_State}", Position );

		if ( force == 0 )
			return;

		var cloudBeClosed = (int)MaxAngle.y == ClosedAngle && (angle <= ClosedAngle || angle.AlmostEqual( ClosedAngle, 2 ));
		var absForce = MathF.Abs( force );

		if ( cloudBeClosed && absForce < 2 && m_State != DoorStates.Closed )
		{
			// Close Door
			m_State = DoorStates.Closed;
			PlayClientEffect( DoorStates.Closed, absForce / 1.4f );

			if ( LastAttackerWeapon == null && LastAttacker is Pawn )
				SetAnimParameter( "open", true );

			return;
		}

		if ( (int)MaxAngle.y == ClosedAngle && angle < ClosedAngle && m_State == DoorStates.Closed )
		{
			// Door is already closed, cant close it anymore
			return;
		}

		if ( (angle > MaxAngle.x || angle < MaxAngle.y) && m_State != DoorStates.Closed )
		{
			// Door is now fully open
			m_Force = force * -1;
			m_State = DoorStates.Bounced;
			PlayClientEffect( DoorStates.Bounced, absForce / 1.4f );

			return;
		}

		if ( !cloudBeClosed && m_State is not DoorStates.Open )
		{
			if ( m_State == DoorStates.Closed )
			{
				if ( LastAttackerWeapon == null && LastAttacker is Pawn )
					SetAnimParameter( "open", true );

				PlayClientEffect( DoorStates.Open, 1 );
			}

			m_State = DoorStates.Open;
		}
	}

	// Client Effects

	private ModelDoorSounds m_Sounds;

	public override void OnNewModel( Model model )
	{
		base.OnNewModel( model );

		if ( model is { IsError: false } && model.TryGetData( out ModelDoorSounds sounds ) )
		{
			m_Sounds = sounds;
			return;
		}

		m_Sounds = new ModelDoorSounds { CloseSound = "break_pottery" };
	}

	[ClientRpc]
	private void PlayClientEffect( DoorStates state, float volume )
	{
		var sound = state switch
		{
			DoorStates.Closed => m_Sounds.CloseSound,
			DoorStates.Open => m_Sounds.OpenSound,
			DoorStates.Bounced => m_Sounds.FullyOpenSound,
			DoorStates.Locked => m_Sounds.LockedSound,
			_ => throw new ArgumentOutOfRangeException( nameof(state), state, null )
		};

		if ( EnabledDoorDebug )
			Log.Info( $"Playing {sound}, from state {state}, with volume {volume}" );

		Sound.FromWorld( sound, Position ).SetVolume( volume );
	}

	// Physics

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		if ( m_State == DoorStates.Closed && SleepWhenClosed )
			return;

		// Don't do anything if its closed
		{
			var force = info.Force.Length / 500;
			Push( info.Position, info.Attacker as Pawn, force );
			LastAttackerWeapon = info.Weapon;
		}
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		base.OnPhysicsCollision( eventData );

		// Don't do anything if its closed

		if ( m_State == DoorStates.Closed && SleepWhenClosed )
			return;

		var mass = eventData.Other.PhysicsShape.Body.Mass;
		var speed = eventData.Speed;

		if ( mass < 20 )
			return;

		var force = speed / 200;
		Push( eventData.Position, null, force );
	}

	public void Push( Vector3 from, Entity invoker, float force )
	{
		if ( !IsAuthority )
			return;

		var direction = Vector3.Dot( (from - Position).Normal, Rotation.Left );
		force *= (direction > 0 ? 1 : -1) * (Inverted ? 1 : -1);
		m_Force += force;

		LastAttacker = invoker;
		LastAttackerWeapon = null;
	}

	public void Push( Entity entity, float force ) => Push( entity.Position, entity, force );
}
