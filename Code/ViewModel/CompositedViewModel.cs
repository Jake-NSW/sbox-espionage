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

		var fov = Screen.CreateVerticalFieldOfView( 64 );
		camera.Attributes.Set( "viewModelFov", fov );
	}

	// Instance

	public Dispatcher Events { get; }

	private readonly IDispatchRegistryTable m_Table;
	private readonly LinkedListNode<CompositedViewModel> m_Node;

	public CompositedViewModel( IDispatchRegistryTable table )
	{
		Game.AssertClient();

		EnableViewmodelRendering = true;

		s_All.AddLast( m_Node = new LinkedListNode<CompositedViewModel>( this ) );

		m_Table = table;
		m_Effects = new HashSet<IViewModelEffect>( 8 );
	}

	protected override void OnDestroy()
	{
		s_All.Remove( m_Node );
	}

	// Effect Stack

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

	public void ImportFrom<T>() where T :struct, ITemplate<CompositedViewModel>
	{
		new T().Generate(this);
	}

	public void Add( IViewModelEffect effect )
	{
		effect.Register( this, m_Table );
		m_Effects.Add( effect );
	}
}
