using Sandbox;

namespace Woosh.Espionage;

public sealed class EquipEntityInteraction : EntityComponent, IEntityInteraction
{
	public InteractionIndicator Indicator => new InteractionIndicator( "Equip", $"{Input.GetButtonOrigin( KEYCODE )} Hold", 0.5f /* replace this with time since.. */ );

	private const string KEYCODE = "use";
	private const float EQUIP_DELAY = 0.55f;

	public bool IsInteractable( Entity entity )
	{
		if ( entity is ISlotted slotted )
		{
			m_SincePressed = 0;
			return Handler.Active != slotted.Slot && entity is ICarriable;
		}

		return false;
	}

	private RealTimeSince m_SincePressed;
	
	private DeployableSlotHandler Handler => Entity.Components.Get<DeployableSlotHandler>();
	private IEntityInventory Inventory => Entity.Components.Get<IEntityInventory>();

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
			Log.Info("Equip Entity");

			var entity = (ISlotted)result.Entity;
			Inventory.Add( result.Entity );
			Handler.Deploy( entity.Slot );
			
			return;
		}

		if ( Input.Released( KEYCODE ) )
		{
			// Failed!
			m_SincePressed = default;
		}
	}
}
