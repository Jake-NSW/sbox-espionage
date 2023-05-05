using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Woosh.Espionage;

public delegate void StructCallback<T>( in T evt ) where T : struct;

public readonly struct ValueChangedEvent<T> : IEvent
{
	public ValueChangedEvent( T value, T old )
	{
		New = value;
		Old = old;
	}

	public T New { get; }
	public T Old { get; }
}

public static class StructEventDispatcherUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Notify<T>( this IDispatchExecutor executor, T value, T old = default )
	{
		executor.Run( new ValueChangedEvent<T>( value, old ) );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void OnValueChanged<T>( this IDispatchRegistryTable table, StructCallback<ValueChangedEvent<T>> cb )
	{
		table.Register( cb );
	}
}

public sealed class StructEventDispatcher : IDispatchRegistryTable, IDispatchExecutor
{
	public StructEventDispatcher()
	{
		m_Registry = new Dictionary<Type, HashSet<Delegate>>();
	}

	// Dispatch

	public void Run<T>( T item ) where T : struct, IEvent
	{
		if ( !m_Registry.TryGetValue( typeof(T), out var stack ) )
		{
			return;
		}

		foreach ( var evt in stack )
		{
			(evt as Action)?.Invoke();
			(evt as StructCallback<T>)?.Invoke( item );
		}
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Run<T>() where T : struct, IEvent => Run( new T() );

	// Registry

	private readonly Dictionary<Type, HashSet<Delegate>> m_Registry;

	public void Register<T>( StructCallback<T> callback ) where T : struct, IEvent => Inject<T>( callback );
	public void Register<T>( Action callback ) where T : struct, IEvent => Inject<T>( callback );

	public void Unregister<T>( Action callback ) where T : struct, IEvent => Erase<T>( callback );
	public void Unregister<T>( StructCallback<T> callback ) where T : struct, IEvent => Erase<T>( callback );

	private void Erase<T>( Delegate callback ) where T : struct, IEvent
	{
		if ( m_Registry.TryGetValue( typeof(T), out var stack ) )
			stack.Remove( callback );
	}

	private void Inject<T>( Delegate callback ) where T : struct, IEvent
	{
		if ( m_Registry.TryGetValue( typeof(T), out var data ) )
		{
			data.Add( callback );
			return;
		}

		m_Registry.Add( typeof(T), new HashSet<Delegate> { callback } );
	}
}
