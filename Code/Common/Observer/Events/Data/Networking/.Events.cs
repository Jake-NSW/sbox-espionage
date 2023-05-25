using Sandbox;
using Woosh.Signals;

namespace Woosh.Common;

public readonly record struct ClientDisconnected( IClient Client ) : ISignal;

public readonly record struct ClientJoined( IClient Client ) : ISignal;
