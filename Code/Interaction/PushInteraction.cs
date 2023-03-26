using Sandbox;

namespace Woosh.Espionage.Interaction;

public sealed class PushInteraction : EntityComponent, IPlayerInteraction
{
	public void Simulate( TraceResult hovering, IClient client )
	{
		if ( Input.MouseWheel != 0 && hovering.Entity is IPushable pushable )
		{
			pushable.Push( Input.MouseWheel );
		}
	}
}
