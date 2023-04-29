﻿using System;
using System.Linq;
using Sandbox;

namespace Woosh.Espionage;

public sealed class GameHud : HudEntity<HudRoot> { }

public partial class Project : GameManager
{
	public static Project Current => GameManager.Current as Project;

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
		Camera?.Update();
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );
		
		var spawn = All.OfType<SpawnPoint>().MinBy( _ => Guid.NewGuid() ).Transform;
		spawn.Position += Vector3.Up * 72;

		var pistol = new Mark23Weapon();
		var pawn = client.Possess<Operator>( spawn );
		pawn.Inventory.Add( pistol );
		pawn.Hands.Deploy( pistol );
	}
}