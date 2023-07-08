using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class AppliedViewModelEntityEffects : ObservableEntityComponent, IEntityEffects
{
	public AnimatedEntity ViewModel { get; }

	public AppliedViewModelEntityEffects( AnimatedEntity viewModel ) {
		ViewModel = viewModel;
	}

	AnimatedEntity IEntityEffects.Target => Entity.IsFirstPersonMode ? ViewModel : null;
}
