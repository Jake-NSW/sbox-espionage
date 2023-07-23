using Sandbox;

namespace Woosh.Espionage;

public sealed class CompositedSceneCameraHelper
{
	public SceneCamera Target { get; }

	public CompositedSceneCameraHelper( SceneCamera camera )
	{
		Game.AssertClient();
		Target = camera;
	}

	// Setup

	public CameraMutateScope Update( InputContext input, IController<CameraSetup> controller = null )
	{
		var setup = new CameraSetup( Target )
		{
			FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView ),
			Hands = new ViewModelSetup()
			{
				Aim = 0,
				Angles = Rotation.Identity,
				Offset = Vector3.Zero,
			}
		};
		
		Build( ref setup, input, controller );
		return new CameraMutateScope( Target, setup );
	}

	// Active

	public IController<CameraSetup> Last => m_Last;
	private IController<CameraSetup> m_Last;

	private void Build( ref CameraSetup setup, in InputContext input, IController<CameraSetup> controller )
	{
		var maybe = controller ?? Find();
		if ( m_Last != maybe )
		{
			m_Last?.Disabled();
			m_Last = maybe;
			m_Last?.Activate( ref setup );
		}

		m_Last?.Update( ref setup, input );
	}

	private static IController<CameraSetup> Find()
	{
		// Pull camera from Pawn if it exists
		if ( Game.LocalPawn is IHave<IController<CameraSetup>> pawn )
			return pawn.Item;

		return null;
	}
}
