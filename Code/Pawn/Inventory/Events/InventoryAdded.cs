using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct InventoryAdded : IEventData
{
	public Entity Entity { get; }

	public InventoryAdded( Entity entity )
	{
		Entity = entity;
	}
}
