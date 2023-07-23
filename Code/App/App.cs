using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

[Title( "App" ), Category("Global"), Icon( "sports_esports" )]
public sealed partial class App : BaseGameManager, IObservable
{
	public static App Current
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get => s_App;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Get<T>() where T : EntityComponent => Current.Components.Get<T>();

	private static App s_App;

	// Instance

	public IDispatcher Events { get; }

	public App()
	{
		Events = new Dispatcher( this, _ => null, _ => null );
		s_App = this;

		// Setup Components
		if ( Game.IsServer )
		{
			Components.Create<CameraBuilder>();
			Components.Create<GamemodeHandlerComponent>();
			Components.Create<ProjectileSimulator>();
		}
	}

	public override void Shutdown()
	{
		if ( s_App == this )
			s_App = null;
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		Events.Run<FrameUpdate>();
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
		Events.Run( new SimulateSnapshot( cl ) );
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		Events.Run( new ClientJoined( client ) );
		var spawn = All.OfType<SpawnPoint>().MinBy( _ => Guid.NewGuid() ).Transform;
		spawn.Position += Vector3.Up * 4;

		var pawn = client.Possess<Operator>( spawn );
		pawn.Inventory.Create<Mk23Firearm>();
		pawn.Inventory.Create<Smg2Firearm>();
		pawn.Slots.Deploy( CarrySlot.Front );
	}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();
		Events.Run<PostLevelLoaded>();
	}

	public override void ClientDisconnect( IClient cl, NetworkDisconnectionReason reason )
	{
		Events.Run( new ClientDisconnected( cl, reason ) );
		base.ClientDisconnect( cl, reason );
	}

	// Drag and Drop

	private async static Task LoadMapFromDragDrop( string url )
	{
		var package = await Package.FetchAsync( url, true );
		if ( package.PackageType == Package.Type.Map )
			Game.ChangeLevel( package.FullIdent );
	}

	public override bool OnDragDropped( string text, Ray ray, string action )
	{
		if ( action == "drop" )
			_ = LoadMapFromDragDrop( text );

		return base.OnDragDropped( text, ray, action );
	}
}
