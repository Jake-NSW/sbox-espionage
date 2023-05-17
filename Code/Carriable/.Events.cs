using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly record struct CreateViewModel( CompositedViewModel ViewModel ) : IEventData;

// Deploying

public readonly record struct DeployingEntity( Entity Entity ) : IEventData;

public readonly record struct DeployedEntity( Entity Entity ) : IEventData;

// Holstering

public readonly record struct HolsteringEntity( Entity Entity, bool Dropped ) : IEventData;

public readonly record struct HolsteredEntity( Entity Entity ) : IEventData;
