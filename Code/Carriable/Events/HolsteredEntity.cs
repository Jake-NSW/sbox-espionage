using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct HolsteredEntity( Entity Entity, Entity Deploying ) : ISignal;
