using Sandbox;

namespace Woosh.Espionage;

public sealed class PushInteraction : EntityComponent, IPlayerInteraction
{
	public void Simulate( TraceResult hovering, IClient client )
	{
		if ( Input.MouseWheel != 0 && hovering.Entity is IPushable pushable )
		{
			pushable.Push( Entity, Input.MouseWheel );
		}
	}
}
