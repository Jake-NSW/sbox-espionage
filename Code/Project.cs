using System;
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

		if ( Game.IsServer )
		{
			_ = new GameHud();
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

		var pistol = new ConfigurableWeapon { Asset = ResourceLibrary.Get<WeaponDataAsset>( "weapons/mk23/mark23.weapon" ) };
		var smg = new ConfigurableWeapon { Asset = ResourceLibrary.Get<WeaponDataAsset>( "weapons/smg2/smg2.weapon" ) };

		var pawn = client.Possess<Operator>( spawn );

		pawn.Inventory.Add( pistol );
		pawn.Inventory.Add( smg );

		pawn.Slots.Assign( 3, pistol );
		pawn.Slots.Assign( 1, smg );
		pawn.Slots.Deploy( 1 );
	}
}
