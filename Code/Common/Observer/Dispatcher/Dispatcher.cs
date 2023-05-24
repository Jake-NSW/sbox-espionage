using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Woosh.Common;

public sealed class Dispatcher : IDisposable, IDispatchRegistryTable, IDispatchExecutor
{
	// Registry

	private readonly Dictionary<Type, HashSet<Delegate>> m_Registry;

	public Dispatcher()
	{
		m_Registry = new Dictionary<Type, HashSet<Delegate>>();
	}

	public void Dispose()
	{
		m_Registry.Clear();
	}

	public bool Any<T>() where T : struct, IEventData
	{
		return Count<T>() > 0;
	}

	public int Count<T>() where T : struct, IEventData
	{
		return m_Registry.TryGetValue( typeof(T), out var items ) ? items.Count : 0;
	}

	// Dispatch

	public void Run<T>( T item, object from = null ) where T : struct, IEventData
	{
		if ( !m_Registry.TryGetValue( typeof(T), out var stack ) )
		{
			return;
		}

		var passthrough = new Event<T>( item, from );
		foreach ( var evt in stack )
		{
			switch ( evt )
			{
				case Action action :
					action.Invoke();
					continue;
				case StructCallback<T> callback :
					callback.Invoke( passthrough );
					continue;
			}

			// Don't do anymore events
			if ( passthrough.Consumed )
				break;
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
