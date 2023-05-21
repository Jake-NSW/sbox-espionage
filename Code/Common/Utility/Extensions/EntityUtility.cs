using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public static class EntityUtility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ImportFrom<T, TTemplate>( this T entity ) where T : class, IEntity where TTemplate : struct, ITemplate<T>
	{
		new TTemplate().Generate( entity );
	}
}
