using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class AppliedViewModelEntityEffects : ObservableEntityComponent, IEntityEffects
{
	public AppliedViewModelEntityEffects( AnimatedEntity target )
	{
		Target = target;
	}

	public AnimatedEntity Target { get; }
}
