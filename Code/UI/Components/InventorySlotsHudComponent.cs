using Sandbox.UI;
using Sandbox.UI.Construct;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class InventorySlotsHudComponent : EntityHudComponent<RootPanel, Pawn>
{
	protected override void OnAutoRegister()
	{
		base.OnAutoRegister();

		Register<SlotAssigned>( OnSlotAssigned );
		Register<SlotDeploying>( OnSlotDeploying );
	}

	private Panel[] m_Slots = new Panel[10];

	protected override void OnCreateUI( RootPanel root )
	{
		base.OnCreateUI( root );
		m_Slots = new Panel[10];
		for ( int i = 0; i < m_Slots.Length; i++ )
		{
			m_Slots[i] = root.Add.Panel( "slot_panel" );
		}
	}

	private void OnSlotAssigned( Event<SlotAssigned> evt )
	{
		Log.Info("Slot Assigned");
		
		if ( m_Slots == null )
			return;

		var panel = m_Slots[evt.Data.Slot];
		panel.DeleteChildren();
		panel.Add.Label( evt.Data.Entity.Info().Name );
	}

	private void OnSlotDeploying( Event<SlotDeploying> evt )
	{
		if ( m_Slots == null )
			return;

		m_Slots[evt.Data.Slot].AddClass( "active" );
	}
}
