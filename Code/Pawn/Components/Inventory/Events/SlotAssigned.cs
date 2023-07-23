using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct SlotAssigned( int Slot, Entity Entity ) : ISignal;
