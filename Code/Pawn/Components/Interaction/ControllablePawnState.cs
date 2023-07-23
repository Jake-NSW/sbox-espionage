using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ControllablePawnState : ObservableEntityComponent<Pawn>, IEntityState<Pawn>, IEntityInteraction, ISingletonComponent, IMutate<CameraSetup>
{
	private Entity m_Target;

	// State

	bool IEntityState<Pawn>.TryEnter()
	{
		return Input.Pressed( App.Actions.Interact ) && m_Target is IControllable;
	}

	bool IEntityState<Pawn>.Simulate( IClient cl )
	{
		// Tab to Leave? Or would it be use?
		return (m_Target is IControllable controllable) && controllable.Simulate( Entity );
	}

	void IEntityState<Pawn>.OnStart()
	{
		// Tell Entity we are Controlling it
		(m_Target as IControllable)?.Entering( Entity );
	}

	void IEntityState<Pawn>.OnFinish()
	{
		(m_Target as IControllable)?.Leaving();
	}

	// Pass-down

	public void OnMutate( ref CameraSetup setup )
	{
		if ( Entity.Machine.Active == this )
			(m_Target as IMutate<CameraSetup>)?.OnMutate( ref setup );
	}

	// Interaction

	InteractionIndicator IEntityInteraction.Indicator => new InteractionIndicator( App.Actions.Interact, Input.GetButtonOrigin( App.Actions.Interact ), 0 );

	bool IEntityInteraction.IsInteractable( Entity entity )
	{
		return entity is IControllable controllable && controllable.IsUsable( Entity );
	}

	void IEntityInteraction.Simulate( in TraceResult result, IClient client )
	{
		m_Target = result.Entity;
	}
}
