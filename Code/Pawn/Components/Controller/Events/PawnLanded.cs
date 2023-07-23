using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct PawnLanded( Vector3 Velocity ) : ISignal;
