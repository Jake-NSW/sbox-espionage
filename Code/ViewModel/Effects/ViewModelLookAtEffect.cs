using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelLookAtEffect : IViewModelEffect
{
	public bool Update( ref ViewModelSetup setup )
	{
		var direction = Screen.GetDirection( Mouse.Position );
		setup.Rotation *= Rotation.LookAt( direction );
		return false;
	}

	public void Register( IDispatchRegistryTable table ) { }
	public void Unregister( IDispatchRegistryTable table ) { }
}
