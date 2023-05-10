using Sandbox;

namespace Woosh.Common.Server;

public readonly struct ClientJoined
{
	public IClient Client { get; }

	public ClientJoined( IClient client )
	{
		Client = client;
	}
}
