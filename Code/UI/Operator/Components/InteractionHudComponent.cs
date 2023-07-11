using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class InteractionHudComponent : EntityHudComponent<PawnEntity>
{
	[Listen]
	private void OnInteractionChanged( Event<InteractionTargetChanged> evt )
	{
		m_Badge?.OnInteractionChanged( evt );
	}

	private UI.InteractionBadge m_Badge;

	protected override Panel OnCreateUI()
	{
		var root = CreateFullscreenPanel();
		root.AddChild( m_Badge = new UI.InteractionBadge() );
		return root;
	}
}
