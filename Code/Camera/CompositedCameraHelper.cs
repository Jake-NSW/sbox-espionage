using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

public sealed class CompositedCameraHelper
{
	public SceneCamera Target { get; }

	public CompositedCameraHelper( SceneCamera camera )
	{
		Game.AssertClient();
		Target = camera;
	}

	// Setup

	public CameraMutateScope Update( InputContext input, ICameraController controller = null )
	{
		var setup = new CameraSetup( Target ) { FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView ) };
		Build( ref setup, input, controller );
		return new CameraMutateScope( Target, setup );
	}

	// Active

	public ICameraController Active => m_Last;
	private ICameraController m_Last;

	private void Build( ref CameraSetup setup, in InputContext input, ICameraController controller )
	{
		var maybe = controller ?? Find();
		if ( m_Last != maybe )
		{
			m_Last?.Disabled();
			m_Last = maybe;
			m_Last?.Enabled( ref setup );
		}

		m_Last?.Update( ref setup, input );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static ICameraController Find()
	{
		// Only allow components to be used as cameras if none was found

		if ( Game.LocalClient.Components.Get<EntityCameraController>() is { } clientCamera )
			return clientCamera;

		if ( Game.LocalPawn?.Components.Get<EntityCameraController>() is { } pawnCamera )
			return pawnCamera;

		return null;
	}
}
