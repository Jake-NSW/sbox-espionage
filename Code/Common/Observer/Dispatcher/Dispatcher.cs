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
		Inject( typeof(T), callback );
	}

	public void Register<T>( Action callback ) where T : struct, IEventData
	{
		Inject( typeof(T), callback );
	}

	public void Unregister<T>( Action callback ) where T : struct, IEventData
	{
		Erase( typeof(T), callback );
	}

	public void Unregister<T>( StructCallback<T> callback ) where T : struct, IEventData
	{
		Erase( typeof(T), callback );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Run<T>() where T : struct, IEventData
	{
		Run( new T() );
	}

	internal void Erase( Type type, Delegate callback )
	{
		if ( m_Registry.TryGetValue( type, out var stack ) )
		{
			stack.Remove( callback );
		}
	}

	internal void Inject( Type type, Delegate callback )
	{
		if ( m_Registry.TryGetValue( type, out var data ) )
		{
			data.Add( callback );
			return;
		}

		m_Registry.Add( type, new HashSet<Delegate> { callback } );
	}
}
