using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class AmmoCheckOverlayHudComponent : EntityHudComponent<Pawn>
{
	protected override Panel OnCreateUI()
	{
		var root = CreateFullscreenPanel();
		m_Overlay = root.AddChild<UI.AmmoCheckOverlay>();
		return root;
	}

	private UI.AmmoCheckOverlay m_Overlay;

	[Listen]
	private void ViewModelCreated( Event<CreatedViewModel> evt )
	{
		m_Overlay.Target = evt.Data.ViewModel;
	}

	[Listen]
	private void OnAmmoCheckOpen( Event<CheckAmmoOpen> evt )
	{
		Log.Info( "Open" );
		m_Overlay.Checking = true;
	}

	[Listen]
	private void OnAmmoCheckClosed( Event<CheckAmmoClosed> evt )
	{
		Log.Info( "Closed" );
		m_Overlay.Checking = false;
	}
}
