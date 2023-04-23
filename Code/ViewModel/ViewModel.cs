using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModel : AnimatedEntity
{
	private readonly static LinkedList<ViewModel> s_All;

	static ViewModel()
	{
		s_All = new LinkedList<ViewModel>();
	}

	[Event.Client.PostCamera]
	private static void UpdateAllViewmodels()
	{
		foreach ( var viewModel in s_All )
		{
			viewModel.Update( new Transform( Camera.Position, Camera.Rotation ) );
		}

		Camera.Main.Attributes.Set( "viewModelFov", Camera.FieldOfView );
	}

	// Instance

	private readonly LinkedListNode<ViewModel> m_Node;

	public ViewModel() : this( null ) { }

	public ViewModel( IEnumerable<IViewModelEffect> effects )
	{
		Game.AssertClient();
		m_Effects = effects != null ? new HashSet<IViewModelEffect>( effects ) : new HashSet<IViewModelEffect>( 8 );

		m_Node = new LinkedListNode<ViewModel>( this );
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
		m_Effects.Add( effect );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Add<T>() where T : IViewModelEffect, new() => Add( new T() );

	public void Remove( IViewModelEffect effect )
	{
		m_Effects.Remove( effect );
	}
}
