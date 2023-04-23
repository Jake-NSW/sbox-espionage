using System;
using Editor;
using Sandbox;
using ModelDoorSounds = Sandbox.DoorEntity.ModelDoorSounds;

namespace Woosh.Espionage;

[Library( "esp_door" ), HammerEntity, Category( "Gameplay" ), Icon( "door_front" ), Model]
public sealed partial class PushableDoorEntity : AnimatedEntity, IPushable
{
	public enum DoorStates : byte
	{
		Closed,
		Open,
		FullyOpen,
		Locked,
	}

	// Utility

	[Property, FGDType( "target_source" )] public string Partner { get; set; }
	[Property] public bool Inverted { get; set; }

	// Angle

	[Property] public Vector2 MaxAngle { get; set; } = new Vector2( 90, 0 );
	[Property] public Vector2 SpawnAngle { get; set; } = new Vector2( 0, 0 );
	[Property] public int ClosedAngle { get; set; } = 0;

	// Physics

	[Property( Title = "Weight (kg)" )] public int Weight { get; set; } = 15;
	[Property] public float Stiffness { get; set; } = 3.5f;

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
	}

	// Debug

	[ConVar.Server( "esp_door_debug" )] private static bool s_DoorDebug;

	// Physics

	public Transform Initial { get; set; }
	private DoorStates m_State;
	private float m_Force;

	[Event.Tick.Server]
	private void Tick()
	{
		Rotation target;

		var maxRot = Initial.Rotation.RotateAroundAxis( Transform.NormalToLocal( Initial.Rotation.Up * (Inverted ? 1 : -1) ), MaxAngle.x );
		var minRot = Initial.Rotation.RotateAroundAxis( Transform.NormalToLocal( Initial.Rotation.Up * (Inverted ? 1 : -1) ), MaxAngle.y );
		var closedRot = Initial.Rotation.RotateAroundAxis( Transform.NormalToLocal( Initial.Rotation.Up * (Inverted ? 1 : -1) ), ClosedAngle );

		// Debug

		if ( s_DoorDebug )
		{
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

		var axis = Transform.NormalToLocal( Initial.Rotation.Up );
		var potential = Rotation.FromAxis( axis, m_Force );
		var dot = Vector3.Dot( Inverted ? (Rotation * potential).Forward : (Rotation * potential).Backward, Initial.Rotation.Left );

		// Angle
		var angle = Rotation.Difference( Rotation * potential, Initial.Rotation ).Angle();
		angle *= dot < 0 ? -1 : 1;

		target = Rotation * potential;

		// Clamp to Axis
		if ( angle > MaxAngle.y && angle < MaxAngle.x )
			Rotation = target;

		if ( s_DoorDebug )
			DebugOverlay.Text( $"Force - {m_Force}\nAngle - {angle}\nDot - {dot}\nRotation - {Rotation.Angles()}", Position );

		if ( m_Force == 0 )
			return;

		var cloudBeClosed = (int)MaxAngle.y == ClosedAngle && angle.AlmostEqual( ClosedAngle, 4 );

		if ( cloudBeClosed && MathF.Abs( m_Force ) < 2 && m_State != DoorStates.Closed )
		{
			// Close Door
			m_State = DoorStates.Closed;
			PlayClientSound( DoorStates.Closed, m_Force / 1.4f );
			
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
			m_Force = m_Force / 2 * -1;
			m_State = DoorStates.FullyOpen;
			PlayClientSound( DoorStates.FullyOpen, m_Force / 1.4f );

			return;
		}

		if ( !cloudBeClosed && m_State != DoorStates.Open )
		{
			if ( m_State == DoorStates.Closed )
			{
				PlayClientSound( DoorStates.Open, 1 );
			}

			m_State = DoorStates.Open;
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

		m_Sounds = new ModelDoorSounds { CloseSound = "break_pottery" };
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

		Log.Info( $"Playing {sound}, from state {state}, with volume {volume}" );
		Sound.FromWorld( sound, Position ).SetVolume( volume );
	}

	// Push

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		// Don't do anything if its closed
		if ( info.Attacker.Position.Distance( Position ) <= 64 || m_State != DoorStates.Closed )
		{
			var force = info.Force.Length / 10;
			Push( info.Position, info.Attacker as Pawn, force );
		}
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		base.OnPhysicsCollision( eventData );
		
		// Don't do anything if its closed
		
		if ( m_State != DoorStates.Closed )
		{
			var force = eventData.Velocity.Length / 40;
			Push( eventData.Position, null, force );
		}
	}

	public void Push( Vector3 from, Entity invoker, float force )
	{
		if ( !IsAuthority )
			return;

		var direction = Vector3.Dot( (from - Position).Normal, Rotation.Left );
		force *= (direction > 0 ? 1 : -1) * (Inverted ? 1 : -1);
		m_Force += force / MathF.Max( 1, Stiffness * Stiffness ) * 8;
		LastAttacker = invoker;
	}

	public void Push( Entity entity, float force ) => Push( entity.Position, entity, force );

}
