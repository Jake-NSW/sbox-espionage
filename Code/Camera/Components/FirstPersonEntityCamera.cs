namespace Woosh.Espionage;

public sealed class FirstPersonEntityCamera : EntityCameraController<Pawn>
{
	protected override void OnUpdate( ref CameraSetup setup, in InputContext input )
	{
		setup.Viewer = Entity;
		setup.Rotation = input.ViewAngles.ToRotation();
		setup.Position = Entity.EyePosition;
	}
}
