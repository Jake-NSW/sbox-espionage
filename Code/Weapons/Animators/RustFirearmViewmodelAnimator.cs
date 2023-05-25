using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class RustFirearmViewmodelAnimator : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	protected override void OnAutoRegister()
	{
		Register<WeaponFired>( OnShoot );
	}

	private void OnShoot()
	{
		Entity.SetAnimParameter( "fire", true );
	}

	public void OnPostCameraSetup( ref CameraSetup setup ) { }
}
