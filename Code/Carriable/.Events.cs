using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct CreatedViewModel( CompositedViewModel ViewModel ) : ISignal;

// Deploying

public readonly record struct DeployingEntity( Entity Entity ) : ISignal;

public readonly record struct DeployedEntity( Entity Entity ) : ISignal;

// Holstering

public readonly record struct HolsteringEntity( Entity Entity, bool Dropped ) : ISignal;

public readonly record struct HolsteredEntity( Entity Entity ) : ISignal;
