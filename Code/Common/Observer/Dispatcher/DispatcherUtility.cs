using System.Runtime.CompilerServices;

namespace Woosh.Common;

public static class DispatcherUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Notify<T>( this IDispatchExecutor executor, T value, T old = default )
	{
		executor.Run( new ValueChanged<T>( value, old ) );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void OnValueChanged<T>( this IDispatchRegistryTable table, StructCallback<ValueChanged<T>> cb )
	{
		table.Register( cb );
	}
}
