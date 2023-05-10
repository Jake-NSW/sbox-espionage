using Sandbox;

namespace Woosh.Common;

public readonly struct ComponentAdded : IEventData
{
	public EntityComponent Component { get; }

	public ComponentAdded( EntityComponent component )
	{
		Component = component;
	}
}
