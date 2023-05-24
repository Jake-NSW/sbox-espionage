using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public static class EntityUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void FromAspect<T, TTemplate>( this T entity, TTemplate template ) where T : class, IEntity where TTemplate : struct, IAspect<T>
	{
		template.Fill( entity );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void FromAspect<T, TTemplate>( this T entity ) where T : class, IEntity where TTemplate : struct, IAspect<T>
	{
		FromAspect( entity, new TTemplate() );
	}
}
