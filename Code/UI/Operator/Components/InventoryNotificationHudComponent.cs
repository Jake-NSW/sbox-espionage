using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class InventoryNotificationHudComponent : EntityHudComponent<UI.OperatorHudRootPanel, Operator>
{
	protected override Panel OnCreateUI()
	{
		var root = CreateFullscreenPanel();

		// Add container which will hold the notifications that will die out after 2 seconds
		var notificationContainer = root.AddChild<Panel>();
		notificationContainer.AddClass( "notification-container" );

		return root;
	}

	[Listen]
	private void OnInventoryAdded( Event<InventoryAdded> evt )
	{
		Log.Info($"Item added to inventory!! {evt.Data.Item}");
	}

	[Listen]
	private void OnInventoryRemoved( Event<InventoryRemoved> evt )
	{
		Log.Info($"Item removed from inventory :(( {evt.Data.Item}");
	}
}
