using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class CompositedCameraBuilder
{
	public SceneCamera Target { get; }

	public CompositedCameraBuilder( SceneCamera camera )
	{
		Game.AssertClient();
		Target = camera;

		m_Effects = new HashSet<ITemporaryCameraEffect>();
	}

	// Setup

	public ICameraController Override { get; set; }

	private readonly HashSet<ITemporaryCameraEffect> m_Effects;

	public void Update( ICameraController controller = null, IMutate<CameraSetup> mutate = null )
	{
		var setup = new CameraSetup( Target ) { FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView ) };
		Build( ref setup, controller ?? Override );

		// Run Effects
		foreach ( var effect in m_Effects.ToArray() )
		{
			if ( effect.OnPostCameraSetup( ref setup ) )
				continue;

			m_Effects.Remove( effect );
		}

		mutate?.OnPostSetup( ref setup );
		
		// Append New Effects
		if ( setup.Effects.Count > 0 )
			m_Effects.UnionWith( setup.Effects );

		// Mutate
		Target.Attributes.Set( "viewModelFov", setup.FieldOfView - 4 );

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
