using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class GamemodeHandlerComponent : ObservableEntityComponent<App>
{
	public IGamemode Active => (IGamemode)n_Active;

	[Net, Change( nameof(OnGamemodeChanged) )]
	private Entity n_Active { get; set; }

	public T Start<T>( Func<T> factory = null ) where T : Entity, IGamemode, new()
	{
		Game.AssertServer();

		// Find Gamemode by Type
		var gamemode = FindGamemodeByType<T>() ?? factory?.Invoke() ?? new T();

		if ( !gamemode.Requesting() )
		{
			Log.Error( $"Couldn't start gamemode \"{TypeLibrary.GetType( typeof(T) ).Name}\"" );
			return null;
		}

		Finish();
		n_Active = gamemode;
		Active?.Started();
		Run( new GamemodeStarted( Active ) );

		return gamemode;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private T FindGamemodeByType<T>() where T : Entity, IGamemode
	{
		return Sandbox.Entity.All.OfType<T>().FirstOrDefault();
	}

	private void OnGamemodeChanged( Entity old, Entity value )
	{
		Game.AssertClient();
		
		if ( old is IGamemode oldGamemode )
			Events.Run( new GamemodeFinished( oldGamemode ) );

		if ( value is IGamemode newGamemode )
			Events.Run( new GamemodeStarted( newGamemode ) );
	}

	public void Finish()
	{
		Game.AssertServer();
		
		Active?.Finished();

		if ( n_Active != null )
			Run( new GamemodeFinished( Active ) );

		n_Active = null;
	}
}
