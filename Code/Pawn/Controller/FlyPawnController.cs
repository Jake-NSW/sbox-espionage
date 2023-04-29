using Sandbox;

namespace Woosh.Espionage;

public sealed class FlyPawnController : PawnController
{
	public override void Simulate( IClient cl )
	{
		var pawn = Entity as Pawn;

		// build movement from the input values
		var movement = pawn.InputDirection.Normal;

		// rotate it to the direction we're facing
		Entity.Velocity = pawn.Rotation * movement;

		// apply some speed to it
		Entity.Velocity *= Input.Down( "run" ) ? 1000 : 200;

		// apply it to our position using MoveHelper, which handles collision
		// detection and sliding across surfaces for us
		MoveHelper helper = new MoveHelper( Entity.Position, Entity.Velocity );
		helper.Trace = helper.Trace.Size( 16 );
		if ( helper.TryMove( Time.Delta ) > 0 )
		{
			Entity.Position = helper.Position;
		}
	}
}
