using Sandbox;

namespace Woosh.Espionage;

public sealed partial class CameraHolderComponent : EntityComponent, ISingletonComponent
{
	public CameraHolderComponent() { }

	public CameraHolderComponent( CameraController camera )
	{
		n_Camera = camera;
	}

	public CameraController Camera => n_Camera;
	[Net] private CameraController n_Camera { get; set; }
}
