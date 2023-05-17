using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly record struct InventoryAdded( Entity Item ) : IEventData;

public readonly record struct InventoryRemoved( Entity Item ) : IEventData;

// Deploying

public readonly record struct DeployingEntity( Entity Entity ) : IEventData;

public readonly record struct DeployedEntity( Entity Entity ) : IEventData;

// Holstering

public readonly record struct HolsteringEntity( Entity Entity, bool Dropped ) : IEventData;

public readonly record struct HolsteredEntity( Entity Entity ) : IEventData;

// Slots

public readonly record struct SlotChanged( int Slot, Entity Entity ) : IEventData;

public readonly record struct SlotDropping( int Slot, Entity Entity ) : IEventData;

public readonly record struct SlotDeploying( int Slot, Entity Entity ) : IEventData;
