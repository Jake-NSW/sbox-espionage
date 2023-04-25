using System;
using System.Linq;
using Sandbox;

namespace Woosh.Espionage;

public sealed class GameHud : HudEntity<HudRoot> { }

public partial class MyGame : GameManager
{
	public MyGame()
	{
		if ( Game.IsServer )
		{
			n_Gamemode = new GamemodeController();
			_ = new GameHud();
		}
	}

	[Net] internal GamemodeController n_Gamemode { get; set; }

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( InputButton.Slot1 ) )
		{
			// Deploy Pistol
			var handler = cl.Pawn.Components.Get<CarriableHandler>();
			var inv = cl.Pawn.Components.Get<IEntityInventory>();

			handler.Deploy(inv.Get<Mark23Weapon>(), 1, 0.6f);
		}
		
		if ( Input.Pressed( InputButton.Slot2 ) )
		{
			// Deploy Pistol
			var handler = cl.Pawn.Components.Get<CarriableHandler>();
			var inv = cl.Pawn.Components.Get<IEntityInventory>();

			handler.Deploy(inv.Get<Smg2Weapon>(), 1.5f, 1.3f);
		}
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new Pawn();
		pawn.Components.Create<PushEntityInteraction>();
		pawn.Components.Create<UseEntityInteraction>();
		pawn.Components.Create<PickupEntityInteraction>();
		var inventory = pawn.Components.Create<InventoryContainer>();

		var gun1 = new Mark23Weapon();
		inventory.Add( gun1 );

		var gun2 = new Smg2Weapon();
		inventory.Add( gun2 );
		
		var handler = pawn.Components.Create<CarriableHandler>();
		handler.Deploy(gun1, 1, 1);

		client.Pawn = pawn;

		// Get all of the spawnpoints
		var spawnpoints = All.OfType<SpawnPoint>();

		// chose a random one
		var randomSpawnPoint = spawnpoints.MinBy( _ => Guid.NewGuid() );

		// if it exists, place the pawn there
		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position += Vector3.Up * 50.0f; // raise it up
			pawn.Transform = tx;
		}
	}
}
