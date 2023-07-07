using Sandbox;

namespace Woosh.Espionage;

public sealed class EventMessageBus : EntityComponent<App>
{
	protected override void OnActivate()
	{
		base.OnActivate();
	}

	protected override void OnCallRemoteProcedure( int id, NetRead read )
	{
		base.OnCallRemoteProcedure( id, read );
	}
}
