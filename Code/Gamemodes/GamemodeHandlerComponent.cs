using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class GamemodeHandlerComponent : ObservableEntityComponent<App>, INetworkSerializer, ISimulated
{
	public IGamemode Active => (IGamemode)n_Active;
	private Entity n_Active { get; set; }

	public T Start<T>( Func<T> factory = null ) where T : Entity, IGamemode
	{
		Game.AssertServer();

		// Find Gamemode by Type
		var gamemode = FindGamemodeByType<T>() ?? factory?.Invoke() ?? TypeLibrary.Create<T>();
		if ( gamemode == null )
		{
			Log.Error( $"Couldn't start gamemode \"{TypeLibrary.GetType( typeof(T) ).Name}\" -- Not valid for this map" );
			return null;
		}

		if ( !gamemode.Requesting() )
		{
			Log.Error( $"Couldn't start gamemode \"{TypeLibrary.GetType( typeof(T) ).Name}\" -- Failed to request" );
			return null;
		}

		Finish();
		n_Active = gamemode;
		Active?.Started();
		Run( new GamemodeStarted( Active ) );

		WriteNetworkData();

		return gamemode;
	}
	
	public void Simulate( IClient client )
	{
		n_Active?.Simulate( client );
	}

	private IEnumerable<IGamemode> m_Gamemodes;

	[Listen]
	private void OnPostLevelLoaded( Event<PostLevelLoaded> evt )
	{
		m_Gamemodes = Sandbox.Entity.All.OfType<IGamemode>();
		Log.Info( "Post Level Loaded - Gamemode!" );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private T FindGamemodeByType<T>() where T : Entity, IGamemode
	{
		return m_Gamemodes.FirstOrDefault( e => e is T ) as T;
	}

	public void Finish()
	{
		Game.AssertServer();

		Active?.Finished();

		if ( n_Active != null )
			Run( new GamemodeFinished( Active ) );

		n_Active = null;
	}

	// Networking

	public void Read( ref NetRead read )
	{
		var ident = read.Read<int>();

		if ( Entity == null || ident == -1 )
			return;

		var entity = Sandbox.Entity.FindByIndex( ident );
		if ( entity == n_Active )
			return;

		Events.Run( new GamemodeFinished( Active ) );
		n_Active = entity;
		Events.Run( new GamemodeStarted( Active ) );
	}

	public void Write( NetWrite write )
	{
		write.Write( n_Active?.NetworkIdent ?? -1 );
	}
}
