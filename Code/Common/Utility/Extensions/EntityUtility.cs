using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public static class EntityUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Import<T, TTemplate>( this T entity, TTemplate template ) where T : class, IEntity where TTemplate : struct, IEntityAspect<T>
	{
		template.ExportTo( entity, entity.Components );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> Build<T>( this T entity ) where T : Entity => new EntityBuilder<T>( entity );
}
