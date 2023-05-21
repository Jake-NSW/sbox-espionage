using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class GenericFirearmViewmodelAnimator : ObservableEntityComponent<Firearm>, IMutateCameraSetup
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
		(Entity?.Effects.Target as AnimatedEntity)?.SetAnimParameter( "bFire", true );
	}

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		(Entity?.Effects.Target as AnimatedEntity)?.SetAnimParameter( "fAimBlend", setup.Hands.Aim );
	}
}
