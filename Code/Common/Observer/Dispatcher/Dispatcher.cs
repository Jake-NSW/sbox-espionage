using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Woosh.Common;

public sealed class Dispatcher : IDispatchRegistryTable, IDispatchExecutor
{
	// Registry

	private readonly Dictionary<Type, HashSet<Delegate>> m_Registry;

	public Dispatcher()
	{
		m_Registry = new Dictionary<Type, HashSet<Delegate>>();
	}

	// Dispatch

	public void Run<T>( T item, object from = null ) where T : struct, IEventData
	{
		if ( !m_Registry.TryGetValue( typeof(T), out var stack ) )
		{
			return;
		}

		foreach ( var evt in stack )
		{
			(evt as Action)?.Invoke();
			(evt as StructCallback<T>)?.Invoke( new Event<T>( item, from ) );
		}
	}

	public void Register<T>( StructCallback<T> callback ) where T : struct, IEventData
	{
		Inject<T>( callback );
	}

	public void Register<T>( Action callback ) where T : struct, IEventData
	{
		Inject<T>( callback );
	}

	public void Unregister<T>( Action callback ) where T : struct, IEventData
	{
		Erase<T>( callback );
	}

	public void Unregister<T>( StructCallback<T> callback ) where T : struct, IEventData
	{
		Erase<T>( callback );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Run<T>() where T : struct, IEventData
	{
		Run( new T() );
	}

	private void Erase<T>( Delegate callback ) where T : struct, IEventData
	{
		if ( m_Registry.TryGetValue( typeof(T), out var stack ) )
		{
			stack.Remove( callback );
		}
	}

	private void Inject<T>( Delegate callback ) where T : struct, IEventData
	{
		if ( m_Registry.TryGetValue( typeof(T), out var data ) )
		{
			data.Add( callback );
			return;
		}

		m_Registry.Add( typeof(T), new HashSet<Delegate> { callback } );
	}
}
