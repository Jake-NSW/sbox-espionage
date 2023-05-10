using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct EntityDeployed : IEventData
{
	public Entity Entity { get; }

	public EntityDeployed( Entity entity )
	{
		Entity = entity;
	}
}
