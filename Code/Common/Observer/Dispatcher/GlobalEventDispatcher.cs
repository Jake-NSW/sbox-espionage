using Sandbox;

namespace Woosh.Common;

public sealed class CallbackAttribute<T> : EventAttribute where T : struct, IEventData
{
	public CallbackAttribute() : base( typeof(T).FullName ) { }
}

public class GlobalEventDispatcher : IDispatchExecutor
{
	public void Run<T>( T item, object from = null ) where T : struct, IEventData
	{
		Event.Run( typeof(T).FullName, new Event<T>( item, from ) );
	}
}
