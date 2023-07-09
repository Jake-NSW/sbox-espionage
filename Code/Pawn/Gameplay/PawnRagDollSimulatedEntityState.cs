using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class PawnRagDollSimulatedEntityState : ObservableEntityComponent<Pawn>, ISimulatedEntityState<Pawn>
{
	private const string INPUT_ACTION = "ragdoll";

	public bool TryEnter()
	{
		return Input.Pressed( INPUT_ACTION );
	}

	public bool Simulate( IClient cl )
	{
		if ( Input.Pressed( "jump" ) )
		{
			return true;
		}

		return false;
	}

	public void OnStart()
	{
		Entity.Camera = new RagDollCamera( Entity );
	}

	public void OnFinish()
	{
		Entity.Camera = null;
	}
}
