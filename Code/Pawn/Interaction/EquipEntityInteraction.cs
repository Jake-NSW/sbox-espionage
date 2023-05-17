using System;
using Sandbox;

namespace Woosh.Espionage;

public sealed class EquipEntityInteraction : EntityComponent<Pawn>, IEntityInteraction, ISingletonComponent
{
	public InteractionIndicator Indicator => new InteractionIndicator(
		"Equip",
		$"{Input.GetButtonOrigin( KEYCODE )} Hold",
		MathF.Min( m_IsActive ? m_SincePressed / EQUIP_DELAY : 0, 1 )
	);

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
	private bool m_IsActive;

	private DeployableSlotHandler Handler => Entity.Components.Get<DeployableSlotHandler>();
	private IEntityInventory Inventory => Entity.Components.Get<IEntityInventory>();

	public void Simulate( in TraceResult result, IClient client )
	{
		using var _ = Prediction.Off();

		if ( Input.Pressed( KEYCODE ) )
		{
			// Start Hold
			m_SincePressed = 0;
			m_IsActive = true;
		}

		if ( m_SincePressed >= EQUIP_DELAY && Input.Down( KEYCODE ) )
		{
			if ( Game.IsServer )
			{
				var entity = (ISlotted)result.Entity;
				Inventory.Add( result.Entity );
				Handler.Deploy( entity.Slot );
			}

			m_IsActive = false;
			return;
		}

		if ( Input.Released( KEYCODE ) )
		{
			// Failed!
			m_SincePressed = default;
			m_IsActive = false;
		}
	}
}
