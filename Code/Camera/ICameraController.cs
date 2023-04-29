namespace Woosh.Espionage;

public readonly ref struct InputContext
{
	public Vector3 ViewAngles { get; }

	public InputContext( Vector3 viewAngles )
	{
		ViewAngles = viewAngles;
	}
}

public interface ICameraController
{
	void Enabled( ref CameraSetup setup );
	void Update( ref CameraSetup setup );
	void Disabled();
}
