using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct PawnPossessed( IClient Client ) : ISignal;
