using Sandbox;
using Sandbox.UI;
using Woosh.Espionage.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class CarriableDeployOverlayComponent : EntityHudComponent<RootPanel, Pawn>
{
	protected override void OnAutoRegister()
	{
		base.OnAutoRegister();
		Register<CreatedViewModel>( OnViewModelCreated );
	}

	private void OnViewModelCreated( Event<CreatedViewModel> evt )
	{
		var ent = evt.Data.Target;

		// Don't worry about it...
		if ( ent == null )
			return;

		var info = ent.Info();
		m_Overlay.Override( info.Name, info.Description, evt.Data.ViewModel );
	}

	private CarriableDeployOverlay m_Overlay;

	protected override void OnCreateUI( RootPanel root )
	{
		m_Overlay = root.AddChild<CarriableDeployOverlay>();
	}
}
