using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct EntityPossessed( IClient Client ) : ISignal;
