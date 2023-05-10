using System.Runtime.CompilerServices;

namespace Woosh.Common;

public interface IDispatchExecutor
{
	public void Run<T>( T item, object from = null ) where T : struct, IEventData;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Run<T>( object from = null ) where T : struct, IEventData
	{
		Run( new T() );
	}
}
