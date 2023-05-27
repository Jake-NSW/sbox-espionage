using Sandbox;
using Sandbox.Utility;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct PawnLanded( Vector3 Velocity ) : ISignal;

public sealed class WalkController : PawnController
{
	private bool IsGrounded => Entity.GroundEntity.IsValid();

	public int StepSize => 24;
	public int WishSpeed => 140;
	public int GroundAngle => 45;
	public int JumpSpeed => 260;
	public float Gravity => 800f;

	private TimeSince m_SinceLanded;
	private bool m_Jumped;

	public override void Simulate( IClient cl )
	{
		var movement = Entity.InputDirection.Normal;
		var angles = Entity.ViewAngles.WithPitch( 0 );
		var moveVector = Rotation.From( angles ) * movement * 320f;
		var groundEntity = CheckForGround();

		if ( groundEntity.IsValid() )
		{
			if ( !IsGrounded )
			{
				Landed();

				m_Jumped = false;
			}

			Entity.Velocity = Accelerate( Entity.Velocity, moveVector.Normal, moveVector.Length, WishSpeed * (Input.Down( "run" ) ? 1.5f : 1f), 8.75f );
			Entity.Velocity = ApplyFriction( Entity.Velocity, 8.0f );

			// Cap our Velocity after we've jumped
			Entity.Velocity *= Easing.Linear( (m_SinceLanded / 1 + 0.35f).Min( 1 ) );
		}
		else
		{
			Entity.Velocity = Accelerate( Entity.Velocity, moveVector.Normal, moveVector.Length, 30, 20f );
			Entity.Velocity += Vector3.Down * Gravity * Time.Delta;
		}

		if ( Input.Pressed( "jump" ) && IsGrounded && m_SinceLanded > 0.4f )
		{
			Velocity = (Entity.Velocity + Vector3.Up * JumpSpeed) * (m_SinceLanded / 0.8f).Min( 1 );
			m_Jumped = true;
		}

		var mh = new MoveHelper( Entity.Position, Entity.Velocity );
		mh.Trace = mh.Trace.Size( Entity.Hull ).Ignore( Entity );

		if ( mh.TryMoveWithStep( Time.Delta, StepSize ) > 0 )
		{
			if ( IsGrounded )
			{
				mh.Position = StayOnGround( mh.Position );
			}

			Entity.Position = mh.Position;
			Entity.Velocity = mh.Velocity;
		}

		Entity.GroundEntity = groundEntity;
	}

	private void Landed()
	{
		Events.Run( new PawnLanded( Velocity ) );

		if ( Velocity.z.Abs() > 250 || m_Jumped )
		{
			Velocity = Velocity.WithZ( 0 );
			m_SinceLanded = 0.2f;
		}
	}

	private Entity CheckForGround()
	{
		if ( Entity.Velocity.z > 100f )
			return null;

		var trace = TraceBBox( Entity.Position, Entity.Position + Vector3.Down, 2f );

		if ( !trace.Hit )
			return null;

		if ( trace.Normal.Angle( Vector3.Up ) > GroundAngle )
			return null;

		return trace.Entity;
	}

	private Vector3 ApplyFriction( Vector3 input, float frictionAmount, float stopSpeed = 100f )
	{
		var speed = input.Length;

		if ( speed < 0.1f )
			return input;

		// Bleed off some speed, but if we have less than the bleed
		// threshold, bleed the threshold amount.
		var control = (speed < stopSpeed) ? stopSpeed : speed;

		// Add the amount to the drop amount.
		var drop = control * Time.Delta * frictionAmount;

		// scale the velocity
		var newSpeed = speed - drop;

		if ( newSpeed < 0 )
			newSpeed = 0;

		if ( newSpeed == speed )
			return input;

		newSpeed /= speed;
		input *= newSpeed;

		return input;
	}

	private Vector3 Accelerate( Vector3 input, Vector3 wishDir, float wishSpeed, float speedLimit, float acceleration )
	{
		if ( speedLimit > 0 && wishSpeed > speedLimit )
			wishSpeed = speedLimit;

		var currentSpeed = input.Dot( wishDir );
		var addSpeed = wishSpeed - currentSpeed;

		if ( addSpeed <= 0 )
			return input;

		var accelSpeed = acceleration * Time.Delta * wishSpeed;

		if ( accelSpeed > addSpeed )
			accelSpeed = addSpeed;

		input += wishDir * accelSpeed;

		return input;
	}

	Vector3 StayOnGround( Vector3 position )
	{
		var start = position + Vector3.Up * 2;
		var end = position + Vector3.Down * StepSize;

		// See how far up we can go without getting stuck
		var trace = TraceBBox( position, start );
		start = trace.EndPosition;

		// Now trace down from a known safe position
		trace = TraceBBox( start, end );

		if ( trace.Fraction <= 0 ) return position;
		if ( trace.Fraction >= 1 ) return position;
		if ( trace.StartedSolid ) return position;
		if ( Vector3.GetAngle( Vector3.Up, trace.Normal ) > GroundAngle ) return position;

		return trace.EndPosition;
	}

	public TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f )
	{
		return TraceBBox( start, end, Entity.Hull.Mins, Entity.Hull.Maxs, liftFeet );
	}

	public TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f )
	{
		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		var tr = Trace.Ray( start, end )
			.Size( mins, maxs )
			.WithAnyTags( "solid", "playerclip", "passbullets" )
			.Ignore( Entity )
			.Run();

		return tr;
	}
}
