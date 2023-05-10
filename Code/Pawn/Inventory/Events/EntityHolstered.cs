using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct EntityHolstered : IEventData
{
	public Entity Entity { get; }
	public bool Dropped { get; }

	public EntityHolstered( Entity entity, bool dropped )
	{
		Entity = entity;
		Dropped = dropped;
	}
}
