namespace Woosh.Espionage;

// Controller

public interface ICameraController
{
	void Enabled( ref CameraSetup setup );
	void Update( ref CameraSetup setup, in InputContext input );
	void Disabled();
}
