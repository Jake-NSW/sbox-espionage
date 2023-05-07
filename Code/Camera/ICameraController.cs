using Sandbox;

namespace Woosh.Espionage;

// Controller

public abstract class EntityCameraController : EntityComponent, ICameraController, ISingletonComponent
{
	public new virtual void Enabled( ref CameraSetup setup ) { }
	public abstract void Update( ref CameraSetup setup );
	public virtual void Disabled() { }
}

public interface ICameraController
{
	void Enabled( ref CameraSetup setup );
	void Update( ref CameraSetup setup );
	void Disabled();
}
