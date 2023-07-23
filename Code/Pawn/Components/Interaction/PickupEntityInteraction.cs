using Sandbox;

namespace Woosh.Espionage;

public sealed class PickupEntityInteraction : EntityComponent<Pawn>, IEntityInteraction
{
	public InteractionIndicator Indicator => new InteractionIndicator( "Pickup", Input.GetButtonOrigin( App.Actions.Interact ), 0 );

	public bool IsInteractable( Entity entity )
	{
		return entity is IPickup;
	}

	private RealTimeSince m_HeldTime;

	public void Simulate( in TraceResult hovering, IClient client )
	{
		if ( Game.IsClient )
			return;

		using var _ = Prediction.Off();

		if ( Input.Pressed( App.Actions.Interact ) )
		{
			m_HeldTime = 0;
		}

		if ( Input.Released( App.Actions.Interact ) && m_HeldTime <= 0.2f )
		{
			Entity.Components.Get<IEntityInventory>().Add( hovering.Entity );
		}
	}
}
