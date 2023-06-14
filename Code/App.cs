using System;
using System.Linq;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class App : GameManager, IObservable
{
	public new static App Current => GameManager.Current as App;
	
	public IDispatcher Events { get; }

	public App()
	{
		Events = new Dispatcher();

		if ( Game.IsServer )
		{
			// Setup Components
			Components.Create<CameraBuilderComponent>();
			Components.Create<GamemodeHandlerComponent>();
		}
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		Events.Run( new FrameUpdate( Time.Delta ) );
	}

	public override void Simulate( IClient cl )
	{
		Components.Each<ISimulated, IClient>( cl, ( client, simulated ) => simulated.Simulate( client ) );
		base.Simulate( cl );
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		var spawn = All.OfType<SpawnPoint>().MinBy( _ => Guid.NewGuid() ).Transform;
		spawn.Position += Vector3.Up * 2;

		var pistol = new Mk23Firearm();
		var smg = new Smg2Firearm();

		var pawn = client.Possess<Operator>( spawn );

		pawn.Inventory.Add( pistol, smg );
		pawn.Slots.Deploy( CarrySlot.Front );
	}
}
