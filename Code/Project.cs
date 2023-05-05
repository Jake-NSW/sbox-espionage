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

		Camera.Update();
		CompositedViewModel.UpdateAllViewModels( Camera.Target );
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		var spawn = All.OfType<SpawnPoint>().MinBy( _ => Guid.NewGuid() ).Transform;
		spawn.Position += Vector3.Up * 72;

		var pistol = new Mk23Firearm();
		var smg = new ConfigurableWeapon { Asset = ResourceLibrary.Get<WeaponDataAsset>( "weapons/smg2/smg2.weapon" ) };

		var pawn = client.Possess<Operator>( spawn );

		pawn.Inventory.Add( pistol );
		pawn.Inventory.Add( smg );

		pawn.Slots.Assign( Operator.CarrySlot.Holster, pistol );
		pawn.Slots.Assign( Operator.CarrySlot.Front, smg );
		pawn.Slots.Deploy( Operator.CarrySlot.Holster );
	}
}
