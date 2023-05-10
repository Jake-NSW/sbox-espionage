using Sandbox;

namespace Woosh.Common.Server;

public readonly struct ClientDisconnected
{
	public IClient Client { get; }

	public ClientDisconnected( IClient client )
	{
		Client = client;
	}
}
