using Sandbox;
using Sandbox.UI;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class OperatorEquipmentHudComponent : EntityHudComponent<UI.OperatorHudRootPanel, Operator>
{
	public InventorySlotHandler Slots => this.Get<InventorySlotHandler>();
	public Entity Active => Entity.Active;

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
		Panel.Parent.Style.Opacity = Entity.Machine.Active == null ? 1 : 0;
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
		if ( evt.Data.Entity == null )
			return;

		// Update the overlay to show the deploying entity
		m_EquipmentDetails.Assign( EntityInfo.FromEntity( evt.Data.Entity ) );
	}

	// Slots

	private UI.InventoryDeployableHotbar m_Hotbar;

	[Listen]
	private void OnSlotAssigned( Event<SlotAssigned> evt )
	{
		if ( evt.Data.Entity == null )
		{
			m_Hotbar.Remove( evt.Data.Slot );
			return;
		}

		m_Hotbar.Assign( evt.Data.Slot, EntityInfo.FromEntity( evt.Data.Entity ) );
	}
	
	[Listen]
	private void OnSlotHolstered( Event<SlotHolstered> evt )
	{
		m_Hotbar.Deploying( 0 );
	}

	[Listen]
	private void OnSlotDeploying( Event<SlotDeploying> evt )
	{
		m_Hotbar.Deploying( evt.Data.Slot );
	}
}
