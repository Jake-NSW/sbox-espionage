namespace Woosh.Espionage;

public interface ICameraController
{
	void Enabled( ref CameraSetup setup );
	void Update( ref CameraSetup setup );
	void Disabled();
}
