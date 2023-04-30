using Sandbox;

namespace Woosh.Espionage;

public struct InteractionIndicator
{
	public string Action { get; }
	public string Bind { get; }
	
	public float Held { get; }
}

public interface IEntityInteraction : IComponent
{
	void Simulate( TraceResult hovering, IClient client );
}
