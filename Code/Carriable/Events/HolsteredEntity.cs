using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct HolsteredEntity( Entity Entity, Entity Deploying ) : ISignal;
