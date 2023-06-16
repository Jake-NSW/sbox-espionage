using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class CarriableDeployOverlayHudComponent : EntityHudComponent<RootPanel, Pawn>
{
	protected override void OnAutoRegister()
	{
		base.OnAutoRegister();

		Register<CreatedViewModel>( OnViewModelCreated );
		Register<DeployedEntity>( OnDeployed );
	}

	private void OnDeployed( Event<DeployedEntity> evt )
	{
		m_Overlay.FadeOut();
	}

	private void OnViewModelCreated( Event<CreatedViewModel> evt )
	{
		var ent = evt.Data.Target;

		// Don't worry about it...
		if ( ent == null )
			return;

		var info = EntityInfo.FromEntity( ent );
		m_Overlay.Override( info.Display, info.Brief, evt.Data.ViewModel );
	}

	private UI.CarriableDeployOverlay m_Overlay;

	protected override void OnCreateUI( RootPanel root )
	{
		m_Overlay = root.AddChild<UI.CarriableDeployOverlay>();
	}
}
