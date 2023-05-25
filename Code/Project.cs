using System;
using System.Linq;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class GameHud : HudEntity<HudRoot> { }

public partial class Project : GameManager
{
	public Project()
	{
		if ( Game.IsClient )
		{
			Camera = new CompositedCameraBuilder( Sandbox.Camera.Main );
		}

		if ( Game.IsServer )
		{
			_ = new GameHud();
		}
	}

	private CompositedCameraBuilder Camera { get; }

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		
		Camera.Update( mutate: Game.LocalPawn as IMutateCameraSetup );
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
