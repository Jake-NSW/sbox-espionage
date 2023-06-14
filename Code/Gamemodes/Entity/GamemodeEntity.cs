using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

[Title( "Gamemode" ), Category( "Gamemodes" ), Icon( "sports_basketball" )]
public abstract class GamemodeEntity : Entity, IGamemode, IObservable
{
	private IDispatcher m_Events;
	public IDispatcher Events => m_Events ??= new Dispatcher( this, _ => App.Current.Events, _ => null );

	bool IGamemode.Requesting() => OnRequesting();
	protected virtual bool OnRequesting() { return true; }


	void IGamemode.Started() { OnStarted(); }
	protected virtual void OnStarted() { }

	void IGamemode.Finished() { OnFinished(); }
	protected virtual void OnFinished() { }
}
