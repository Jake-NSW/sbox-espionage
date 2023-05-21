using Sandbox;

namespace Woosh.Espionage;

public sealed class WalkController : PawnController
{
	private bool IsGrounded => Entity.GroundEntity.IsValid();

	public int StepSize => 24;
	public int GroundAngle => 45;
	public int JumpSpeed => 300;
	public float Gravity => 800f;

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
				Entity.Velocity = Entity.Velocity.WithZ( 0 );
			}

			Entity.Velocity = Accelerate( Entity.Velocity, moveVector.Normal, moveVector.Length, 200.0f * (Input.Down( "run" ) ? 2.5f : 1f), 7.5f );
			Entity.Velocity = ApplyFriction( Entity.Velocity, 8.0f );
		}
		else
		{
			Entity.Velocity = Accelerate( Entity.Velocity, moveVector.Normal, moveVector.Length, 100, 20f );
			Entity.Velocity += Vector3.Down * Gravity * Time.Delta;
		}

		if ( Input.Pressed( "jump" ) )
		{
			DoJump();
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

	void DoJump()
	{
		if ( IsGrounded )
		{
			Entity.Velocity = ApplyJump( Entity.Velocity, "jump" );
		}
	}

	Entity CheckForGround()
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

	Vector3 ApplyFriction( Vector3 input, float frictionAmount )
	{
		float StopSpeed = 100.0f;

		var speed = input.Length;
		if ( speed < 0.1f ) return input;

		// Bleed off some speed, but if we have less than the bleed
		// threshold, bleed the threshold amount.
		float control = (speed < StopSpeed) ? StopSpeed : speed;

		// Add the amount to the drop amount.
		var drop = control * Time.Delta * frictionAmount;

		// scale the velocity
		float newSpeed = speed - drop;
		if ( newSpeed < 0 ) newSpeed = 0;
		if ( newSpeed == speed ) return input;

		newSpeed /= speed;
		input *= newSpeed;

		return input;
	}

	Vector3 Accelerate( Vector3 input, Vector3 wishdir, float wishspeed, float speedLimit, float acceleration )
	{
		if ( speedLimit > 0 && wishspeed > speedLimit )
			wishspeed = speedLimit;

		var currentspeed = input.Dot( wishdir );
		var addspeed = wishspeed - currentspeed;

		if ( addspeed <= 0 )
			return input;

		var accelspeed = acceleration * Time.Delta * wishspeed;

		if ( accelspeed > addspeed )
			accelspeed = addspeed;

		input += wishdir * accelspeed;

		return input;
	}

	Vector3 ApplyJump( Vector3 input, string jumpType )
	{
		return input + Vector3.Up * JumpSpeed;
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
