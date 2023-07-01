namespace Woosh.Espionage;

public sealed class FirstPersonEntityCamera : EntityCameraController
{
	public override void Update( ref CameraSetup setup )
	{
		setup.Viewer = Entity;
		setup.Rotation = Rotation.LookAt( Entity.AimRay.Forward );
		setup.Position = Entity.AimRay.Position;
	}
}
