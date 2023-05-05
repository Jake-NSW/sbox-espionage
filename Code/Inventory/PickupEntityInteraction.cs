using Sandbox;

namespace Woosh.Espionage;

public sealed class EquipEntityInteraction : EntityComponent, IEntityInteraction
{
	public InteractionIndicator Indicator => new InteractionIndicator( "Equip", $"{Input.GetButtonOrigin( KEYCODE )} Hold", 0.5f /* replace this with time since.. */ );

	private const string KEYCODE = "use";
	private const float EQUIP_DELAY = 1.5f;

	public bool IsInteractable( Entity entity )
	{
		return entity is ICarriable;
	}

	private RealTimeSince m_SincePressed;

	public void Simulate( in TraceResult result, IClient client )
	{
		if ( Game.IsClient )
			return;

		using var _ = Prediction.Off();

		if ( Input.Pressed( KEYCODE ) )
		{
			m_SincePressed = 0;
		}

		if ( m_SincePressed >= EQUIP_DELAY )
		{
			// go and equip da gun....
			return;
		}

		if ( Input.Released( KEYCODE ) )
		{
			// Failed!
			m_SincePressed = default;
		}
	}
}

public sealed class PickupEntityInteraction : EntityComponent, IEntityInteraction
{
	public InteractionIndicator Indicator => new InteractionIndicator( "Pickup", $"{Input.GetButtonOrigin( KEYCODE )}", 0 );

	private const string KEYCODE = "use";

	public bool IsInteractable( Entity entity )
	{
		return entity is IPickup;
	}

	public void Simulate( in TraceResult hovering, IClient client )
	{
		if ( Game.IsClient )
			return;

		using var _ = Prediction.Off();

		if ( Input.Released( KEYCODE ) )
		{
			Entity.Components.Get<IEntityInventory>().Add( hovering.Entity );
		}
	}
}
