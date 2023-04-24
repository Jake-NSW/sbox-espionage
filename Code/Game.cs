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

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new Pawn();
		pawn.Components.Add( new PushInteraction() );
		pawn.Components.Add( new UseInteraction() );
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
