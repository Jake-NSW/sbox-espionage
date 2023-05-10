using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct InventoryRemoved : IEventData
{
	public Entity Entity { get; }

	public InventoryRemoved( Entity entity )
	{
		Entity = entity;
	}
}
