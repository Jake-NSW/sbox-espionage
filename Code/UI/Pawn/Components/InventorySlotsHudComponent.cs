using Sandbox.UI;
using Sandbox.UI.Construct;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class InventorySlotsHudComponent : EntityHudComponent<Pawn>
{
	public DeployableSlotHandler Slots => this.Get<DeployableSlotHandler>();

	private Panel[] m_Slots = new Panel[10];
	
	protected override void OnCreateUI( Panel root )
	{
		base.OnCreateUI( root );
		m_Slots = new Panel[10];
		for ( int i = 0; i < m_Slots.Length; i++ )
		{
			m_Slots[i] = root.Add.Panel( "slot_panel" );
		}
	}

	[Listen]
	private void OnSlotAssigned( Event<SlotAssigned> evt )
	{
		if ( m_Slots == null )
		{
			Log.Info( "Slots are null!" );
			return;
		}

		var panel = m_Slots[evt.Data.Slot];
		panel.DeleteChildren();

		string name;

		if ( evt.Data.Entity == null )
			name = "Unknown";
		else
			name = evt.Data.Entity.Info().Name;
		
		panel.Add.Label( name );
	}

	[Listen]
	private void OnSlotDeploying( Event<SlotDeploying> evt )
	{
		if ( m_Slots == null )
			return;

		m_Slots[evt.Data.Slot].AddClass( "active" );
	}
}
