using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct FirearmFired( Vector3 Recoil, Vector3 Kickback ) : ISignal;
