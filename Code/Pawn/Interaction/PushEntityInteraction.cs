using Sandbox;

namespace Woosh.Espionage;

public interface IPushable
{
	void Push( Entity entity, float force );
}

public sealed class PushEntityInteraction : EntityComponent, IEntityInteraction, ISingletonComponent
{
	public InteractionIndicator[] Interaction => new[] { new InteractionIndicator( "Push & Pull", "Scroll", 0 ) };

	public void Simulate( TraceResult hovering, IClient client )
	{
		if ( Input.MouseWheel != 0 && hovering.Entity is IPushable pushable )
		{
			pushable.Push( Entity, Input.MouseWheel );
		}
	}
}
