using Sandbox;

namespace Woosh.Espionage;

public sealed class FirstPersonCamera : ICameraController
{
	private readonly Entity m_Owner;

	public FirstPersonCamera( Entity owner )
	{
		m_Owner = owner;
	}

	public void Enabled( ref CameraSetup setup ) { }

	public void Update( ref CameraSetup setup )
	{
		setup.Viewer = m_Owner;
		setup.Rotation = m_ViewAngles.ToRotation();
		setup.Position = m_Owner.Position;
	}

	private Angles m_ViewAngles;

	public void Feed( in InputContext context )
	{
		m_ViewAngles = context.ViewAngles;
	}

	public void Disabled() { }
}

public sealed class FirstPersonEntityCamera : EntityCameraController
{
	public override void Update( ref CameraSetup setup )
	{
		setup.Viewer = Entity;
		setup.Rotation = m_ViewAngles.ToRotation();
		setup.Position = Entity.Position;
		
		Entity.Components.Get<PawnLeaning>()?.OnCameraSetup(ref setup);
	}

	private Angles m_ViewAngles;

	public void Feed( in InputContext context )
	{
		m_ViewAngles = context.ViewAngles;
	}
}
