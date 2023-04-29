using Sandbox;

namespace Woosh.Espionage;

public readonly ref struct InputContext
{
	public Angles ViewAngles { get; }

	public InputContext( Angles viewAngles )
	{
		ViewAngles = viewAngles;
	}
}

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
