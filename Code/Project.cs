using System;
using System.Linq;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class Project : GameManager
{
	public Project()
	{
		if ( Game.IsClient )
		{
			Camera = new CompositedCameraBuilder( Sandbox.Camera.Main );
		}
	}

	private CompositedCameraBuilder Camera { get; }

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );

		Camera.Update( mutate: Game.LocalPawn as IMutate<CameraSetup> );
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		var spawn = All.OfType<SpawnPoint>().MinBy( _ => Guid.NewGuid() ).Transform;
		spawn.Position += Vector3.Up * 72;

		var pistol = new Mk23Firearm();
		var smg = new Smg2Firearm();

		var pawn = client.Possess<Operator>( spawn );

		pawn.Inventory.Add( pistol );
		pawn.Inventory.Add( smg );

		pawn.Slots.Assign( CarrySlot.Holster, pistol );
		pawn.Slots.Assign( CarrySlot.Front, smg );
	}
}
