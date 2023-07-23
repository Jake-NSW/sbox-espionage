using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class RustFirearmViewModelAnimator : ObservableEntityComponent<ViewModel>, IViewModelEffect
{
	protected override void OnAutoRegister()
	{
		Register<FirearmFired>( () => Entity.SetAnimParameter( "fire", true ) );
		Register<DeployingEntity>( () => Entity.SetAnimParameter( "deploy", true ) );
	}

	void IMutate<CameraSetup>.OnMutate( ref CameraSetup setup ) { }
}
