using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class RustFirearmViewmodelAnimator : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	protected override void OnAutoRegister()
	{
		Register<WeaponFired>( () => Entity.SetAnimParameter( "fire", true ) );
		Register<DeployingEntity>( () => Entity.SetAnimParameter( "deploy", true ) );
	}

	public void OnPostCameraSetup( ref CameraSetup setup ) { }
}
