using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public interface IViewModelEffect : IMutateCameraSetup
{
	void Register( AnimatedEntity model, IDispatchRegistryTable table ) { }
	void Unregister( IDispatchRegistryTable table ) { }
}
