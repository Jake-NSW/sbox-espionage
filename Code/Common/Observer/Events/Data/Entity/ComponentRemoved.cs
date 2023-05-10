using Sandbox;

namespace Woosh.Common;

public readonly struct ComponentRemoved : IEventData
{
	public EntityComponent Component { get; }

	public ComponentRemoved( EntityComponent component )
	{
		Component = component;
	}
}
