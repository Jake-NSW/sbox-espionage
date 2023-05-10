using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct HolsteringEntity : IEventData
{
	public Entity Entity { get; }
	public bool Dropped { get; }

	public HolsteringEntity( Entity entity, bool dropped )
	{
		Entity = entity;
		Dropped = dropped;
	}
}

public readonly struct HolsteredEntity : IEventData
{
	public Entity Entity { get; }

	public HolsteredEntity( Entity entity )
	{
		Entity = entity;
	}
}
