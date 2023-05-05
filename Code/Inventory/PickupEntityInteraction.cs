using Sandbox;

namespace Woosh.Espionage;

public sealed class EquipEntityInteraction : EntityComponent, IEntityInteraction
{
	public InteractionIndicator Indicator => new InteractionIndicator( "Equip", $"{Input.GetButtonOrigin( KEYCODE )} Hold", 0.5f /* replace this with time since.. */ );

	private const string KEYCODE = "use";
	private const float EQUIP_DELAY = 0.55f;

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
			// Start Hold
			m_SincePressed = 0;
		}

		if ( m_SincePressed >= EQUIP_DELAY && Input.Down( KEYCODE ) )
		{
			// go and equip da gun....
			Log.Info( "Equip Gun" );
			Entity.Components.Get<IEntityInventory>().Add( result.Entity );
			Entity.Components.Get<CarriableHandler>().Deploy( result.Entity );
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

	private RealTimeSince m_HeldTime;

	public void Simulate( in TraceResult hovering, IClient client )
	{
		if ( Game.IsClient )
			return;

		using var _ = Prediction.Off();

		if ( Input.Pressed( KEYCODE ) )
		{
			m_HeldTime = 0;
		}

		if ( Input.Released( KEYCODE ) && m_HeldTime <= 0.2f )
		{
			Log.Info( "Picking Up" );
			Entity.Components.Get<IEntityInventory>().Add( hovering.Entity );
		}
	}
}
