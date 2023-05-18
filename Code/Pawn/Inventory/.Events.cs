using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly record struct InventoryAdded( Entity Item ) : IEventData;

public readonly record struct InventoryRemoved( Entity Item ) : IEventData;


// Slots

public readonly record struct SlotAssigned( int Slot, Entity Entity ) : IEventData;

public readonly record struct SlotDropping( int Slot, Entity Entity ) : IEventData;

public readonly record struct SlotDeploying( int Slot, Entity Entity ) : IEventData;
