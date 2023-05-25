using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class RustFirearmViewmodelAnimator : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	protected override void OnActivate()
	{
		Events.Register<WeaponFired>( OnShoot );
	}

	protected override void OnDeactivate()
	{
		Events.Unregister<WeaponFired>( OnShoot );
	}

	private void OnShoot()
	{
		Entity.SetAnimParameter( "fire", true );
	}

	public void OnPostCameraSetup( ref CameraSetup setup ) { }
}
