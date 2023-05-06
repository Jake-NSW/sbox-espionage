using Sandbox;

namespace Woosh.Espionage;

public interface IViewModelEffect : IMutateCameraSetup
{
	void Register( AnimatedEntity model, IDispatchRegistryTable table ) { }
	void Unregister( IDispatchRegistryTable table ) { }
}
