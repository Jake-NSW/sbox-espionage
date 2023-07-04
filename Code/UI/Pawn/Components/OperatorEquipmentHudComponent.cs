using Sandbox;
using Sandbox.UI;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class OperatorEquipmentHudComponent : EntityHudComponent<UI.OperatorHudRootPanel, Operator>
{
	public DeployableSlotHandler Slots => this.Get<DeployableSlotHandler>();
	public Entity Active => Entity.Active;

	protected override Panel OnCreateUI()
	{
		var root = new Panel();
		
		root.AddClass( "equipment-panel" );
		m_EquipmentDetails = root.AddChild<UI.ActiveEquipmentDetails>();
		m_Hotbar = root.AddChild<UI.InventoryDeployableHotbar>();

		return root;
	}

	// Active

	private UI.ActiveEquipmentDetails m_EquipmentDetails;

	[Listen]
	private void OnDeploying( Event<DeployingEntity> evt )
	{
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
	private void OnSlotDeploying( Event<SlotDeploying> evt )
	{
		m_Hotbar.Deploying( evt.Data.Slot );
	}
}
