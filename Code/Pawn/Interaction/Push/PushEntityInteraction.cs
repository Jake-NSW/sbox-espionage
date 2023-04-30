using Sandbox;

namespace Woosh.Espionage;

public sealed class PushEntityInteraction : EntityComponent, IEntityInteraction, ISingletonComponent
{
	public bool TryInteract( Entity ent )
	{
		return ent is IPushable;
	}

	public void Simulate( TraceResult hovering, IClient client )
	{
		if ( Input.MouseWheel != 0 && hovering.Entity is IPushable pushable )
		{
			pushable.Push( Entity, Input.MouseWheel );
		}
	}
}
