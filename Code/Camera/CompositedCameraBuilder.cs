using Sandbox;

namespace Woosh.Espionage;

public sealed class CompositedCameraBuilder
{
	private readonly SceneCamera m_Camera;

	public CompositedCameraBuilder( SceneCamera camera )
	{
		m_Camera = camera;
		Event.Register( this );
	}

	~CompositedCameraBuilder()
	{
		Event.Unregister( this );
	}

	public void Update( ICameraController controller = null )
	{
		var setup = new CameraSetup( m_Camera );

		Build( ref setup, controller );

		m_Camera.Position = setup.Position;
		m_Camera.Rotation = setup.Rotation;

		m_Camera.Size = setup.Viewport;
		m_Camera.FirstPersonViewer = setup.Viewer;

		m_Camera.FieldOfView = setup.FieldOfView;
	}

	private ICameraController m_Last;

	private void Build( ref CameraSetup setup, ICameraController controller )
	{
		var maybe = controller ?? Find();
		if ( m_Last != maybe )
		{
			m_Last?.Disabled();
			m_Last = maybe;
			m_Last?.Enabled( ref setup );
		}

		m_Last?.Update( ref setup );
	}

	private ICameraController Find()
	{
		if ( GameManager.Current is IActive<ICameraController> game )
			return game.Active;

		if ( Game.LocalPawn is IActive<ICameraController> pawn )
			return pawn.Active;

		return null;
	}
}
