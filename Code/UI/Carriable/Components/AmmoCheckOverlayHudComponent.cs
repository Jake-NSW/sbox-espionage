using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class AmmoCheckOverlayHudComponent : EntityHudComponent<Pawn>
{
	protected override void OnCreateUI( Panel root )
	{
		base.OnCreateUI( root );
		m_Overlay = root.AddChild<UI.AmmoCheckOverlay>();
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
