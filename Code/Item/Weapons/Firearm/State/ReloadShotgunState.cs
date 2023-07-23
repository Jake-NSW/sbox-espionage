using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class ReloadShotgunState : ObservableEntityComponent<Firearm>, IEntityState<Firearm>, ISingletonComponent
{
	private TimeSince m_SinceInput;

	public bool TryEnter()
	{
		if ( Input.Pressed( "reload" ) )
		{
			m_SinceInput = 0;
			return false;
		}

		if ( Input.Released( "reload" ) && m_SinceInput <= 0.15f )
		{
			m_SinceInput = default;
			return true;
		}

		return false;
	}

	[Net, Predicted, Local] private TimeSince n_SinceReload { get; set; }

	public bool Simulate( IClient cl )
	{
		Log.Info("reloading");
		
		// We're Done!
		return n_SinceReload > 3;
	}

	public void OnStart()
	{
		n_SinceReload = 0;

		Run( new ReloadStarted() );
	}

	public void OnFinish() { }
}
