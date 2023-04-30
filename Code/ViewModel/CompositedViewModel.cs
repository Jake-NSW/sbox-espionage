using System.Collections.Generic;
using Sandbox;

namespace Woosh.Espionage;

[Title( "View Model" ), Category( "ViewModel" ), Icon( "pan_tool" )]
public sealed class CompositedViewModel : AnimatedEntity
{
	private readonly static LinkedList<CompositedViewModel> s_All;

	static CompositedViewModel()
	{
		s_All = new LinkedList<CompositedViewModel>();
	}

	public static void UpdateAllViewModels( SceneCamera camera )
	{
		foreach ( var viewModel in s_All )
		{
			viewModel.Update( new Transform( camera.Position, camera.Rotation ) );
		}

		var fov = Screen.CreateVerticalFieldOfView( 65 );
		camera.Attributes.Set( "viewModelFov", fov );
	}

	// Instance

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

	private void Update( Transform origin )
	{
		Position = origin.Position;
		Rotation = origin.Rotation;

		var setup = new ViewModelSetup( this, Owner, origin, GetAnimParameterFloat( "fAimBlend" ) );

		foreach ( var effect in m_Effects )
		{
			effect.Update( ref setup );
		}

		Position = setup.Position;
		Rotation = setup.Rotation;
	}

	public void Add( IViewModelEffect effect )
	{
		effect.Register( m_Table );
		m_Effects.Add( effect );
	}
}
