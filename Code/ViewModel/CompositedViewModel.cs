using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sandbox;
using Sandbox.UI;

namespace Woosh.Espionage;

[Category( "ViewModel" )]
[Title( "View Model" ), Icon( "pan_tool" )]
public sealed class CompositedViewModel : AnimatedEntity
{
	private readonly static LinkedList<CompositedViewModel> s_All;

	static CompositedViewModel()
	{
		s_All = new LinkedList<CompositedViewModel>();
	}

	[Event.Client.PostCamera]
	private static void UpdateAllViewModels()
	{
		foreach ( var viewModel in s_All )
		{
			viewModel.Update( new Transform( Camera.Position, Camera.Rotation ) );
		}
		
		Camera.Main.Attributes.Set( "viewModelFov", Camera.FieldOfView );
	}

	// Instance

	private readonly IDispatchRegistryTable m_Table;
	private readonly LinkedListNode<CompositedViewModel> m_Node;

	public CompositedViewModel( IDispatchRegistryTable table, IEnumerable<IViewModelEffect> effects = null )
	{
		Game.AssertClient();

		m_Table = table;
		m_Effects = effects != null ? new HashSet<IViewModelEffect>( effects ) : new HashSet<IViewModelEffect>( 8 );

		m_Node = new LinkedListNode<CompositedViewModel>( this );
		s_All.AddLast( m_Node );

		EnableViewmodelRendering = true;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		s_All.Remove( m_Node );
	}

	// Effect Stack

	private readonly HashSet<IViewModelEffect> m_Effects;

	private void Update( Transform origin )
	{
		var setup = new ViewModelSetup( Owner, origin, 0 );

		foreach ( var effect in m_Effects )
		{
			effect.Update( ref setup );
		}

		// Update Self

		Position = setup.Position;
		Rotation = setup.Rotation;
	}

	public void Add( IViewModelEffect effect )
	{
		effect.Register( m_Table );
		m_Effects.Add( effect );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Add<T>() where T : IViewModelEffect, new() => Add( new T() );

	public void Remove( IViewModelEffect effect )
	{
		m_Effects.Remove( effect );
		effect.Unregister( m_Table );
	}
}
