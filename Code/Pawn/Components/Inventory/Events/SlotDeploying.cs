using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct SlotDeploying( int Slot, Entity Entity ) : ISignal;
