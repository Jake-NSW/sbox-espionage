using Sandbox;

namespace Woosh.Espionage;

public interface IEntityInteraction : IComponent
{
	void Simulate( TraceResult hovering, IClient client );
}
