namespace Woosh.Espionage;

public interface IViewModelEffect
{
	bool Update( ref ViewModelSetup setup );

	void Register( IDispatchRegistryTable table ) { }
	void Unregister( IDispatchRegistryTable table ) { }
}
