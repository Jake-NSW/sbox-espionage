using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct InventoryAdded( Entity Item ) : ISignal;
