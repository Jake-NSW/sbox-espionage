using Sandbox;

namespace Woosh.Espionage;

public struct InteractionIndicator
{
	public InteractionIndicator( string action, string bind, float held )
	{
		Action = action;
		Bind = bind;
		Held = held;
	}

	public string Action { get; }
	public string Bind { get; }
	public float Held { get; }
}

public interface IEntityInteraction : IComponent
{
	InteractionIndicator[] Interaction { get; }
	void Simulate( TraceResult hovering, IClient client );
}
