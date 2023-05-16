using Sandbox;

namespace Woosh.Espionage;

public interface IEntityEffects : IComponent
{
	public ModelEntity Target { get; }
}
