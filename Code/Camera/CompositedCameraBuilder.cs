using Sandbox;

namespace Woosh.Espionage;

public sealed class CompositedCameraBuilder
{
	public SceneCamera Target { get; }

	public CompositedCameraBuilder( SceneCamera camera )
	{
		Game.AssertClient();
		Target = camera;
	}

	// Setup

	public ICameraController Override { get; set; }

	public void Update( ICameraController controller = null )
	{
		var setup = new CameraSetup( Target ) { FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView ) };

		Build( ref setup, controller ?? Override );

		// Mutate

		Target.Position = setup.Position;
		Target.Rotation = setup.Rotation;
		Target.FirstPersonViewer = setup.Viewer;
		Target.FieldOfView = setup.FieldOfView;
	}

	// Active

	public ICameraController Active => m_Last;
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

		if ( Game.LocalPawn.Components.Get<EntityCameraController>() is { } component )
			return component;

		return null;
	}
}
