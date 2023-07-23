using Sandbox;
using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class InventoryNotificationHudComponent : EntityHudComponent<UI.OperatorHudRootPanel, Operator>
{
	protected override Panel OnCreateUI()
	{
		var root = CreateFullscreenPanel();

		// Add container which will hold the notifications that will die out after 2 seconds
		m_Container = root.Add.Panel( "notification-mask" ).AddChild<Panel>();
		m_Container.AddClass( "notification-container" );

		return root;
	}

	private Panel m_Container;

	[Listen]
	private void OnInventoryAdded( Event<InventoryAdded> evt )
	{
		Add( EntityInfo.FromEntity( evt.Signal.Item ), false );
	}

	private async void Add( EntityInfo info, bool removed )
	{
		const float delay = 2.4f;

		var element = new UI.InventoryNotificationOverlay( info, removed );
		m_Container.AddChild( element );
		await GameTask.DelaySeconds( delay );
		element.Delete();
	}

	[Listen]
	private void OnInventoryRemoved( Event<InventoryRemoved> evt )
	{
		Add( EntityInfo.FromEntity( evt.Signal.Item ), true );
	}
}
