using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class FallPawnState : ObservableEntityComponent<Pawn>, IEntityState<Pawn>
{
	public bool TryEnter()
	{
		return Input.Pressed( App.Actions.Fall );
	}

	private TimeSince m_SinceEntered;

	public bool Simulate( IClient cl )
	{
		if ( Input.Pressed( "jump" ) && m_SinceEntered > 1.9f )
		{
			return true;
		}

		return false;
	}

	public void OnStart()
	{
		m_SinceEntered = 0;
		Entity.Camera = new RagDollCamera();
	}

	public void OnFinish()
	{
		Entity.Camera = null;
	}
}
