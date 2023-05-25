using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct InventoryAdded( Entity Item ) : ISignal;

public readonly record struct InventoryRemoved( Entity Item ) : ISignal;


// Slots

public readonly record struct SlotAssigned( int Slot, Entity Entity ) : ISignal;

public readonly record struct SlotDropping( int Slot, Entity Entity ) : ISignal;

public readonly record struct SlotDeploying( int Slot, Entity Entity ) : ISignal;
