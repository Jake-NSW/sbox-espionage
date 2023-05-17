using Sandbox;

namespace Woosh.Espionage;

public interface IControllable : IEntity
{
	bool IsUsable( Entity pawn );

	void Entering( Entity pawn );
	void Leaving();

	bool Simulate( Entity pawn );
}

public sealed class ControllableEntityInteraction : EntityComponent<Pawn>, IEntityInteraction, ISingletonComponent
{
	public InteractionIndicator Indicator => new InteractionIndicator( "Use", Input.GetButtonOrigin( "use" ), 0 );

	public bool IsInteractable( Entity entity )
	{
		return entity is IControllable;
	}

	public void Simulate( in TraceResult result, IClient client ) { }
}
