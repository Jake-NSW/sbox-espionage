using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public interface IControllable : IEntity
{
	bool IsUsable( Entity pawn );

	void Entering( Entity pawn );
	void Leaving();

	bool Simulate( Entity pawn );
}

public sealed class ControllableEntityInteraction : EntityComponent<Pawn>, IEntityInteraction, ISingletonComponent, ISimulatedEntityState<Pawn>
{
	public InteractionIndicator Indicator => new InteractionIndicator( "Use", Input.GetButtonOrigin( "use" ), 0 );

	public bool IsInteractable( Entity entity )
	{
		return entity is IControllable;
	}

	public void Simulate( in TraceResult result, IClient client ) { }
	
	// Simulation State
	
	public bool TryEnter()
	{
		return false;
	}

	public bool Simulate( IClient cl )
	{
		return true;
	}

	public void OnStart()
	{
		throw new System.NotImplementedException();
	}

	public void OnFinish()
	{
		throw new System.NotImplementedException();
	}
}
