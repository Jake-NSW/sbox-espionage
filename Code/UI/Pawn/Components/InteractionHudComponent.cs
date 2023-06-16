using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class InteractionHudComponent : EntityHudComponent<RootPanel, Pawn>
{
	[Listen]
	private void OnInteractionChanged( Event<InteractionTargetChanged> evt )
	{
		m_Badge?.OnInteractionChanged( evt );
	}

	private UI.InteractionBadge m_Badge;

	protected override void OnCreateUI( RootPanel root )
	{
		root.AddChild( m_Badge = new UI.InteractionBadge() );
	}
}
