using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

public sealed partial class TestGamemode : Entity, IGamemode
{
	public bool Requesting()
	{
		Log.Info( "Requesting to start gamemode" );
		return true;
	}

	public void Started()
	{
		Log.Info( "Starting gamemode" );
	}

	public void Finished()
	{
		Log.Info( "Finished gamemode gamemode" );
	}
}

public sealed partial class GamemodeController : BaseNetworkable
{
	public IGamemode Active => (IGamemode)n_Active;

	[Net] private Entity n_Active { get; set; }

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

		Active?.Finished();
		n_Active = gamemode;
		Active?.Started();

		return gamemode;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public T FindGamemodeByType<T>() where T : Entity, IGamemode
	{
		return Entity.All.OfType<T>().FirstOrDefault();
	}

	public void Finish()
	{
		Active.Finished();
		n_Active = null;
	}
}
