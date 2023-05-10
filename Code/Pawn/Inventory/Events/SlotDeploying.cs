using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct SlotChanged : IEventData
{
	public int Slot { get; }
	public Entity Entity { get; }

	public SlotChanged( int slot, Entity entity )
	{
		Slot = slot;
		Entity = entity;
	}
}

public readonly struct SlotDropping : IEventData
{
	public int Slot { get; }
	public Entity Entity { get; }

	public SlotDropping( int slot, Entity entity )
	{
		Slot = slot;
		Entity = entity;
	}
}

public readonly struct SlotDeploying : IEventData
{
	public int Slot { get; }
	public Entity Entity { get; }

	public SlotDeploying( int slot, Entity entity )
	{
		Slot = slot;
		Entity = entity;
	}
}
