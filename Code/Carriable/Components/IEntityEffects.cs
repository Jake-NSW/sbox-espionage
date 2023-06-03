using Sandbox;

namespace Woosh.Espionage;

public interface IEntityEffects : IComponent
{
	public AnimatedEntity Target { get; }
}
