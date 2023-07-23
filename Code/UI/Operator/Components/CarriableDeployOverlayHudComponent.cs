using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class CarriableDeployOverlayHudComponent : EntityHudComponent<Pawn>
{
	[Listen]
	private void OnDeployed( Event<DeployedEntity> evt )
	{
		m_Overlay.FadeOut();
	}

	[Listen]
	private void OnViewModelCreated( Event<CreatedViewModel> evt )
	{
		var ent = evt.Signal.Target;

		// Don't worry about it...
		if ( ent == null )
			return;

		var info = EntityInfo.FromEntity( ent );
		m_Overlay.Override( info, evt.Signal.ViewModel );
	}

	private UI.CarriableDeployOverlay m_Overlay;

	protected override Panel OnCreateUI()
	{
		var root = CreateFullscreenPanel();
		m_Overlay = root.AddChild<UI.CarriableDeployOverlay>();
		return root;
	}
}
