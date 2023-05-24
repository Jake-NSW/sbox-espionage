using System;
using System.Linq;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class GameHud : HudEntity<HudRoot> { }

public partial class Project : GameManager, IMutateCameraSetup
{
	public static Project Current => GameManager.Current as Project;

	public IDispatchExecutor Events { get; }

	public Project()
	{
		Events = new GlobalEventDispatcher();

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
		Camera.Update( mutate: this );

		Events.Run( new FrameUpdate( Time.Delta ) );
	}

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		(Game.LocalPawn as IMutateCameraSetup)?.OnPostCameraSetup( ref setup );
		CompositedViewModel.UpdateAllViewModels( ref setup, Camera.Target );
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

		pawn.Hands.Assign( CarrySlot.Holster, pistol );
		pawn.Hands.Assign( CarrySlot.Front, smg );
	}
}
