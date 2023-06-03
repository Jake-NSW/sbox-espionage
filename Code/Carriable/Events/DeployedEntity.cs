using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct DeployedEntity( Entity Entity ) : ISignal;
