using Sandbox;

namespace Woosh.Espionage;

public sealed class FlyPawnController : PawnController
{
	public override void Simulate( IClient cl )
	{
		// build movement from the input values
		var movement = Entity.Input.InputDirection.Normal;

		// rotate it to the direction we're facing
		Velocity = Rotation * movement;

		// apply some speed to it
		Velocity *= Input.Down( "run" ) ? 1000 : 200;

		// apply it to our position using MoveHelper, which handles collision
		// detection and sliding across surfaces for us
		MoveHelper helper = new MoveHelper( Position, Velocity );
		helper.Trace = helper.Trace.Size( 16 );
		if ( helper.TryMove( Time.Delta ) > 0 )
		{
			Position = helper.Position;
		}
	}
}
