using Sandbox.UI;
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

	private void OnSlotAssigned( Event<SlotAssigned> evt ) { }

	private void OnSlotDeploying( Event<SlotDeploying> evt ) { }
}
