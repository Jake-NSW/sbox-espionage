using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelBreatheEffect : ObservableEntityComponent<ViewModel>, IViewModelEffect
{
	public void OnMutate( ref CameraSetup setup ) { }
}
