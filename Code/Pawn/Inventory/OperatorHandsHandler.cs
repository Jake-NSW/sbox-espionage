using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class OperatorHandsHandler : ObservableEntityComponent<Operator>
{
	[Net] public PlayerHands Hands { get; set; }

	protected override void OnActivate()
	{
		base.OnActivate();
		
		if ( Game.IsServer )
			Hands ??= new PlayerHands { Owner = Entity, Parent = Entity };
	}

	[Listen]
	private void OnHolstered( Event<HolsteredEntity> evt )
	{
		// Don't call deploy on client!!!!
		if ( Game.IsClient )
			return;

		if ( evt.Data.Deploying == null && evt.Data.Deploying is not PlayerHands )
			Entity.Carriable.Deploy( Hands );
	}
}
