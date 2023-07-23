using Sandbox;
using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class OperatorEquipmentHudComponent : EntityHudComponent<UI.OperatorHudRootPanel, Pawn>
{
	protected override Panel OnCreateUI()
	{
		var root = new Panel();

		root.AddClass( "equipment-panel" );
		m_EquipmentDetails = root.AddChild<UI.ActiveEquipmentDetails>();
		m_Hotbar = root.AddChild<UI.InventoryDeployableHotbar>();

		return root;
	}

	[GameEvent.Client.Frame]
	private void Frame()
	{
		// Hide UI if the pawn is in a state
		Panel.Parent.Style.Opacity = Entity.Machine.InState ? 0 : 1;
	}

	// Active

	private UI.ActiveEquipmentDetails m_EquipmentDetails;

	[Listen]
	private void OnHolstered( Event<HolsteredEntity> evt )
	{
		m_EquipmentDetails.Remove();
	}

	[Listen]
	private void OnDeploying( Event<DeployingEntity> evt )
	{
		if ( evt.Signal.Entity == null )
			return;

		// Update the overlay to show the deploying entity
		m_EquipmentDetails.Assign( EntityInfo.FromEntity( evt.Signal.Entity ) );
	}

	// Slots

	private UI.InventoryDeployableHotbar m_Hotbar;

	[Listen]
	private void OnSlotAssigned( Event<SlotAssigned> evt )
	{
		if ( evt.Signal.Entity == null )
		{
			m_Hotbar.Remove( evt.Signal.Slot );
			return;
		}

		m_Hotbar.Assign( evt.Signal.Slot, EntityInfo.FromEntity( evt.Signal.Entity ) );
	}

	[Listen]
	private void OnSlotHolstered( Event<SlotHolstered> evt )
	{
		m_Hotbar.Deploying( 0 );
	}

	[Listen]
	private void OnSlotDeploying( Event<SlotDeploying> evt )
	{
		m_Hotbar.Deploying( evt.Signal.Slot );
	}
}
