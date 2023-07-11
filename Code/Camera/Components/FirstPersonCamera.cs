using Sandbox;

namespace Woosh.Espionage;

public sealed class FirstPersonCamera : ICameraController
{
	public PawnEntity Entity { get; }

	public FirstPersonCamera( PawnEntity entity )
	{
		Entity = entity;
	}

	void ICameraController.Enabled( ref CameraSetup setup ) { }
	void ICameraController.Disabled() { }

	public void Update( ref CameraSetup setup, in InputContext input )
	{
		setup.Viewer = Entity;
		setup.Rotation = input.ViewAngles.ToRotation();
		setup.Position = Entity.EyePosition;
	}
}

public sealed class FirstPersonEntityCamera : EntityComponent<PawnEntity>, IEntityCameraController
{
	private ICameraController m_Impl;

	protected override void OnActivate()
	{
		m_Impl = new FirstPersonCamera( Entity );
	}

	void ICameraController.Enabled( ref CameraSetup setup )
	{
		m_Impl.Enabled( ref setup );
	}

	public void Update( ref CameraSetup setup, in InputContext input )
	{
		m_Impl.Update( ref setup, input );
	}

	public void Disabled()
	{
		m_Impl.Disabled();
	}
}
