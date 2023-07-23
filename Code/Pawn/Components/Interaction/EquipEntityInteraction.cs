using System;
using Sandbox;

namespace Woosh.Espionage;

public sealed class EquipEntityInteraction : EntityComponent<Pawn>, IEntityInteraction, ISingletonComponent
{
	private readonly static string s_Bind = $"{Input.GetButtonOrigin( App.Actions.Interact )} Hold";

	public InteractionIndicator Indicator => new InteractionIndicator(
		"Equip", s_Bind, MathF.Min( m_IsActive ? m_SincePressed / EQUIP_DELAY : 0, 1 )
	);

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

	private TimeSince m_SincePressed;
	private bool m_IsActive;

	private CarrySlotHandler Handler => Entity.Components.Get<CarrySlotHandler>();
	private IEntityInventory Inventory => Entity.Components.Get<IEntityInventory>();

	public void Simulate( in TraceResult result, IClient client )
	{
		using var _ = Prediction.Off();

		if ( Input.Pressed( App.Actions.Interact ) )
		{
			// Start Hold
			m_SincePressed = 0;
			m_IsActive = true;
		}

		if ( m_SincePressed >= EQUIP_DELAY && Input.Down( App.Actions.Interact ) )
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

		if ( Input.Released( App.Actions.Interact ) )
		{
			// Failed!
			m_SincePressed = default;
			m_IsActive = false;
		}
	}
}
