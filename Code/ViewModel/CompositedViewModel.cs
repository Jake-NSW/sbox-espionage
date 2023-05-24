using System.Collections.Generic;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Title( "View Model" ), Category( "ViewModel" ), Icon( "pan_tool" )]
public sealed class CompositedViewModel : AnimatedEntity, IObservableEntity, IMutateCameraSetup
{
	public Dispatcher Events { get; }

	public CompositedViewModel( IObservableEntity parent )
	{
		Game.AssertClient();

		EnableViewmodelRendering = true;

		Events = parent.Events;
		m_Effects = new HashSet<IViewModelEffect>( 8 );
	}

	// Effect Stack

	protected override void OnComponentAdded( EntityComponent component )
	{
		base.OnComponentAdded( component );

		if ( component is IViewModelEffect effect )
			m_Effects.Add( effect );
	}

	protected override void OnComponentRemoved( EntityComponent component )
	{
		base.OnComponentRemoved( component );

		if ( component is IViewModelEffect effect )
			m_Effects.Remove( effect );
	}

	private readonly HashSet<IViewModelEffect> m_Effects;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		Position = setup.Position + setup.Hands.Offset;
		Rotation = setup.Rotation * setup.Hands.Angles;

		var hands = setup.Hands;

		foreach ( var effect in m_Effects )
		{
			effect.OnPostCameraSetup( ref setup );
		}

		// Append Effects
		Position = setup.Position + setup.Hands.Offset;
		Rotation = setup.Rotation * setup.Hands.Angles;

		setup.Hands = hands;
	}
}
