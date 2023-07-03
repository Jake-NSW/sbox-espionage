using Sandbox.UI;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class InventorySlotsHudComponent : EntityHudComponent<Pawn>
{
	public DeployableSlotHandler Slots => this.Get<DeployableSlotHandler>();

	protected override void OnCreateUI( Panel root )
	{
		base.OnCreateUI( root );
		m_Overlay = root.AddChild<UI.InventoryHotbarOverlay>();
	}

	private UI.InventoryHotbarOverlay m_Overlay;

	[Listen]
	private void OnSlotAssigned( Event<SlotAssigned> evt )
	{
		if ( evt.Data.Entity == null )
		{
			m_Overlay.Remove( evt.Data.Slot );
			return;
		}

		m_Overlay.Assign( evt.Data.Slot, EntityInfo.FromEntity( evt.Data.Entity ) );
	}

	[Listen]
	private void OnSlotDeploying( Event<SlotDeploying> evt )
	{
		m_Overlay.Deploying( evt.Data.Slot );
	}
}
