namespace Woosh.Espionage;

public sealed class ViewModelAutoAimAdjustEffect : IViewModelEffect
{
	public ViewModelAutoAimAdjustEffect( Vector3 position ) { }

	public bool Update( ref ViewModelSetup setup )
	{
		return false;
	}

	public void Register( IDispatchRegistryTable table ) { }
	public void Unregister( IDispatchRegistryTable table ) { }
}
