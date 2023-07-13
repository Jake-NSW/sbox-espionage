using System;
using Sandbox;

namespace Woosh.Espionage;

public struct CameraMutateScope : IDisposable
{
	private readonly SceneCamera m_Camera;

	public CameraMutateScope( SceneCamera camera, CameraSetup setup )
	{
		m_Camera = camera;
		m_Setup = setup;
	}

	private CameraSetup m_Setup;

	public void Mutate( IPostMutate<CameraSetup> mutate )
	{
		mutate?.OnPostMutate( ref m_Setup );
	}

	public void Dispose()
	{
		m_Camera.Attributes.Set( "viewModelFov", m_Setup.FieldOfView );
		m_Camera.Position = m_Setup.Position;
		m_Camera.Rotation = m_Setup.Rotation;
		m_Camera.FirstPersonViewer = m_Setup.Viewer;
		m_Camera.FieldOfView = m_Setup.FieldOfView;
	}
}
