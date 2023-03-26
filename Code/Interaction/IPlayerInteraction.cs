using Sandbox;

namespace Woosh.Espionage.Interaction;

public interface IPlayerInteraction
{
	void Simulate( TraceResult hovering, IClient client );
}
