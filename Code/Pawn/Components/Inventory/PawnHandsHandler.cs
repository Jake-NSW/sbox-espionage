using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class PawnHandsHandler : ObservableEntityComponent<Pawn>
{
	[Net] public PlayerHands Hands { get; set; }

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Game.IsServer )
		{
			Hands ??= new PlayerHands
			{
				Owner = Entity,
				Parent = Entity
			};
		}
	}

	[Listen]
	private void OnHolstered( Event<HolsteredEntity> evt )
	{
		// Don't call deploy on client!!!!
		if ( Game.IsClient )
			return;

		if ( evt.Signal.Deploying == null && evt.Signal.Deploying is not PlayerHands )
			Entity.Components.Get<CarriableHandler>().Deploy( Hands );
	}
}
