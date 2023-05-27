using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ControllableSimulatedEntityState : ObservableEntityComponent<Pawn>, ISimulatedEntityState<Pawn>, ISingletonComponent
{
	public bool TryEnter()
	{
		return Input.Pressed( "use" );
	}

	public bool Simulate( IClient cl )
	{
		// Tab to Leave? Or would it be use?
		return true;
	}

	public void OnStart()
	{
		// Tell Entity we are Controlling it
		(Entity.Components.Get<InteractionHandler>().Hovering as IControllable)?.Entering( Entity );
	}

	public void OnFinish()
	{
		(Entity.Components.Get<InteractionHandler>().Hovering as IControllable)?.Leaving();
	}
}
