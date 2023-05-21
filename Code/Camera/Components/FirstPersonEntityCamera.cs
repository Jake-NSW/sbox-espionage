namespace Woosh.Espionage;
public sealed class FirstPersonEntityCamera : EntityCameraController, IMutateInputContext
{
	public override void Update( ref CameraSetup setup )
	{
		setup.Viewer = Entity;
		setup.Rotation = m_ViewAngles.ToRotation();
		setup.Position = Entity.AimRay.Position;
	}

	private Angles m_ViewAngles;

	public void OnPostInputBuild( ref InputContext context )
	{
		m_ViewAngles = context.ViewAngles;
	}
}
