using Sandbox;

namespace Woosh.Common;

public readonly struct SimulateSnapshot
{
	public IClient Client { get; }

	public SimulateSnapshot( IClient client )
	{
		Client = client;
	}
}
