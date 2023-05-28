namespace Woosh.Espionage;

public sealed class FirstPersonEntityCamera : EntityCameraController, IMutate<InputContext>
{
	public override void Update( ref CameraSetup setup )
	{
		setup.Viewer = Entity;
		setup.Rotation = m_ViewAngles.ToRotation();
		setup.Position = Entity.AimRay.Position;
	}

	private Angles m_ViewAngles;

	public void OnPostSetup( ref InputContext setup )
	{
		m_ViewAngles = setup.ViewAngles;
	}
}
