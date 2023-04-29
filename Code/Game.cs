using System;
using System.Linq;
using Sandbox;

namespace Woosh.Espionage;

public sealed class GameHud : HudEntity<HudRoot> { }

public partial class MyGame : GameManager
{
	public MyGame()
	{
		if ( Game.IsClient )
		{
			Camera = new CompositedCameraBuilder( Sandbox.Camera.Main );
		}
	}

	private CompositedCameraBuilder Camera { get; }

	[GameEvent.Client.PostCamera]
	private void PostClientCamera()
	{
		Camera?.Update();
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new Pawn();
		pawn.Components.Create<PushEntityInteraction>();
		pawn.Components.Create<UseEntityInteraction>();
		pawn.Components.Create<PickupEntityInteraction>();
		pawn.Components.Create<InteractionHandler>();
		pawn.Components.Create<CarriableHandler>();

		var inv = pawn.Components.Create<InventoryContainer>();
		inv.Add( new Mark23Weapon() );

		client.Pawn = pawn;

		var spawnpoints = All.OfType<SpawnPoint>();
		var randomSpawnPoint = spawnpoints.MinBy( _ => Guid.NewGuid() );

		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position += Vector3.Up * 50.0f; // raise it up
			pawn.Transform = tx;
		}
	}
}
