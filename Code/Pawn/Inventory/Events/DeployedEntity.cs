using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct DeployingEntity : IEventData
{
	public Entity Entity { get; }

	public DeployingEntity( Entity entity )
	{
		Entity = entity;
	}
}

public readonly struct DeployedEntity : IEventData
{
	public Entity Entity { get; }

	public DeployedEntity( Entity entity )
	{
		Entity = entity;
	}
}
