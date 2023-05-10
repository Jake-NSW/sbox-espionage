using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public static class ComponentUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static TComponent Get<TComponent>( this EntityComponent component ) where TComponent : class, IComponent => component.Entity.Components.Get<TComponent>();

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static IEnumerable<IComponent> All( this IComponentSystem system )
	{
		return system.GetAll<IComponent>();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T GetAny<T>( this IComponentSystem components )
	{
		return (T)components.GetAll<IComponent>().FirstOrDefault( e => e is T );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool HasAny<T>( this IComponentSystem components ) where T : IComponent
	{
		return components.Get<T>() != null;
	}
}
