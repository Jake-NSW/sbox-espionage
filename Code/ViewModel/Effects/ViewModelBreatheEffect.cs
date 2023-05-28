using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelBreatheEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public void OnPostSetup( ref CameraSetup setup ) { }
}
