using System;
using Sandbox;

namespace Woosh.Espionage;

public interface IPushable
{
	void Push( Entity entity, float force );
}

public sealed class PushEntityInteraction : EntityComponent, IEntityInteraction, ISingletonComponent
{
	public InteractionIndicator Indicator => new InteractionIndicator( "Push", "Scroll", MathF.Abs(Input.MouseWheel) / 14 );

	public bool IsInteractable( Entity entity )
	{
		return entity is IPushable;
	}

	public void Simulate( in TraceResult hovering, IClient client )
	{
		if ( Input.MouseWheel != 0 && hovering.Entity is IPushable pushable )
		{
			pushable.Push( Entity, Input.MouseWheel );
		}
	}
}
