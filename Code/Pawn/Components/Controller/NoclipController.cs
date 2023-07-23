using Sandbox;

namespace Woosh.Espionage;

public sealed class NoclipController : PawnController
{
	public override void Simulate( IClient cl )
	{
		var fwd = Entity.Input.InputDirection.x.Clamp( -1f, 1f );
		var left = Entity.Input.InputDirection.y.Clamp( -1f, 1f );
		var rotation = Entity.EyeRotation;

		var vel = (rotation.Forward * fwd) + (rotation.Left * left);

		if ( Input.Down( "jump" ) )
			vel += Vector3.Up * 1;

		vel = vel.Normal * 2000;

		if ( Input.Down( "run" ) )
			vel *= 5.0f;

		if ( Input.Down( "duck" ) )
			vel *= 0.2f;

		Velocity += vel * Time.Delta;

		if ( Velocity.LengthSquared > 0.01f )
			Position += Velocity * Time.Delta;

		Velocity = Velocity.Approach( 0, Velocity.Length * Time.Delta * 5.0f );
		Entity.GroundEntity = null;
	}
}
