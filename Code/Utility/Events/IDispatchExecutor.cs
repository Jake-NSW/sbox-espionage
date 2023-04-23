using System.Runtime.CompilerServices;

namespace Woosh.Espionage;

public interface IDispatchExecutor
{
	public void Run<T>( T item ) where T : struct, IEvent;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Run<T>() where T : struct, IEvent => Run( new T() );
}
