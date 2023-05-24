using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class RustFirearmViewmodelAnimator : ObservableEntityComponent<Firearm>, IViewModelEffect
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
		(Entity?.Effects.Target as AnimatedEntity)?.SetAnimParameter( "fire", true );
	}

	public void OnPostCameraSetup( ref CameraSetup setup ) { }
}
