using Sandbox;

namespace Woosh.Common;

public readonly record struct ClientDisconnected( IClient Client ) : IEventData;

public readonly record struct ClientJoined( IClient Client ) : IEventData;
