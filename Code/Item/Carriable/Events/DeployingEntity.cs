using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct DeployingEntity( Entity Entity ) : ISignal;
