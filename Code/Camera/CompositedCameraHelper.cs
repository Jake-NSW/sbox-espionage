using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Common;

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
		// Pull Camera from Client if it exists
		if ( Game.LocalClient.Components.Get<IEntityCameraController>() is { } clientCamera )
			return clientCamera;

		// Pull camera from Pawn if it exists
		if ( Game.LocalPawn is IHave<ICameraController> { Item: not null } pawn )
			return pawn.Item;

		// Pull from Pawn if camera exists
		if ( Game.LocalPawn?.Components.Get<IEntityCameraController>() is { } pawnCamera )
			return pawnCamera;

		return null;
	}
}
