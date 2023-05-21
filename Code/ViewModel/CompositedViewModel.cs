using System.Collections.Generic;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Title( "View Model" ), Category( "ViewModel" ), Icon( "pan_tool" )]
public sealed class CompositedViewModel : AnimatedEntity, IObservableEntity
{
	private readonly static LinkedList<CompositedViewModel> s_All;

	static CompositedViewModel()
	{
		s_All = new LinkedList<CompositedViewModel>();
	}

	public static void UpdateAllViewModels( ref CameraSetup setup, SceneCamera camera )
	{
		foreach ( var viewModel in s_All )
		{
			viewModel.Update( ref setup );
		}

		camera.Attributes.Set( "viewModelFov", setup.FieldOfView - 4 );
	}

	// Instance

	public Dispatcher Events { get; }

	private readonly LinkedListNode<CompositedViewModel> m_Node;

	public CompositedViewModel( IObservableEntity parent )
	{
		Game.AssertClient();

		EnableViewmodelRendering = true;

		s_All.AddLast( m_Node = new LinkedListNode<CompositedViewModel>( this ) );

		Events = parent.Events;
		m_Effects = new HashSet<IViewModelEffect>( 8 );
	}

	protected override void OnDestroy()
	{
		s_All.Remove( m_Node );
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

	private void Update( ref CameraSetup setup )
	{
		// Apply Twice

		Position = setup.Position + setup.Hands.Offset;
		Rotation = setup.Rotation * setup.Hands.Angles;

		foreach ( var effect in m_Effects )
		{
			effect.OnPostCameraSetup( ref setup );
		}

		// Append Effects
		Position = setup.Position + setup.Hands.Offset;
		Rotation = setup.Rotation * setup.Hands.Angles;
	}
}
