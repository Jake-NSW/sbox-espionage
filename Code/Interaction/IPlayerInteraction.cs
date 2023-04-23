using Sandbox;

namespace Woosh.Espionage;

public interface IPlayerInteraction
{
	void Simulate( TraceResult hovering, IClient client );
}
