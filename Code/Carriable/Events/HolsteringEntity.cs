using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct HolsteringEntity( Entity Entity, bool Dropped ) : ISignal;
