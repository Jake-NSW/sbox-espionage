using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class App : GameManager, IObservable
{
	public new static App Current
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get => GameManager.Current as App;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Get<T>() where T : EntityComponent
	{
		return Current.Components.Get<T>();
	}

	// Commands

	[ConCmd.Admin( "esp_ent_create" )]
	public static void CreateEntity( string className )
	{
		var caller = ConsoleSystem.Caller;
		var forward = caller.Pawn.AimRay.Forward;
		var start = caller.Pawn.AimRay.Position;
		var ray = Trace.Ray( start, start + forward * 128 ).Ignore( caller.Pawn ).Run();
		TypeLibrary.GetType( className ).Create<Entity>().Position = ray.EndPosition + -forward * 4;
	}

	// Instance

	public IDispatcher Events { get; }

	public App()
	{
		Events = new Dispatcher();

		// Setup Components
		if ( Game.IsServer )
		{
			Components.Create<CameraBuilderComponent>();
			Components.Create<GamemodeHandlerComponent>();
			Components.Create<ProjectileSimulator>();
		}
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		Events.Run<FrameUpdate>();
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
		var hands = new PlayerHands();

		var pawn = client.Possess<Operator>( spawn );
		hands.Owner = pawn;

		pawn.Inventory.Add( pistol, smg );
		pawn.Slots.Assign( CarrySlot.Utility, hands );
		pawn.Slots.Deploy( CarrySlot.Front );
	}
}
