using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

[Title( "View Model" ), Category( "ViewModel" ), Icon( "pan_tool" )]
public sealed class CompositedViewModel : AnimatedEntity, IObservable, IMutate<CameraSetup>, IMutate<InputContext>
{
	public IDispatcher Events { get; }

	public CompositedViewModel( IObservable parent )
	{
		Game.AssertClient();

		EnableViewmodelRendering = true;

		Events = parent.Events;
		m_Effects = new List<IViewModelEffect>( 8 );
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

	private readonly List<IViewModelEffect> m_Effects;

	void IMutate<CameraSetup>.OnMutate( ref CameraSetup setup )
	{
		if ( setup.Viewer == null )
			return;

		var initialPos = setup.Hands.Offset;
		var initialRot = setup.Hands.Angles;

		Position = setup.Position + setup.Hands.Offset;
		Rotation = setup.Rotation * setup.Hands.Angles;

		var hands = setup.Hands;

		foreach ( var effect in m_Effects )
		{
			effect.OnMutate( ref setup );
		}

		// Append Effects
		Position += setup.Hands.Offset - initialPos;
		Rotation *= setup.Hands.Angles * initialRot.Inverse;

		setup.Hands = hands;
	}

	void IMutate<InputContext>.OnMutate( ref InputContext setup )
	{
		foreach ( var input in Components.All().OfType<IMutate<InputContext>>() )
		{
			input.OnMutate( ref setup );
		}
	}
}
