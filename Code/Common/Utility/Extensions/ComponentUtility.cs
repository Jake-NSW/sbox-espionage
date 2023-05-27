using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public static class ComponentUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Each<TComponent, TValue>( this IComponentSystem system, TValue value, Action<TValue, TComponent> loop )
	{
		foreach ( var component in system.All() )
		{
			if(component is TComponent cast)
				loop.Invoke(value, cast);
		}
	}
	
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Each<TComponent>( this IComponentSystem system, Action<TComponent> loop )
	{
		foreach ( var component in system.All() )
		{
			if(component is TComponent cast)
				loop.Invoke(cast);
		}
	}
	
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static TComponent Get<TComponent>( this EntityComponent component ) where TComponent : class, IComponent => component.Entity.Components.Get<TComponent>();

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static IEnumerable<IComponent> All( this IComponentSystem system )
	{
		return system.GetAll<IComponent>();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static IComponent GetAny( this IComponentSystem components, Type type )
	{
		return components.GetAll<IComponent>().FirstOrDefault( e => e.GetType() == type );
	}

	public static IComponent GetOrCreateAny( this IComponentSystem system, Type type )
	{
		if ( system.GetAny( type ) is { } component )
			return component;

		if ( TypeLibrary.GetType( type ).IsInterface )
			return null;

		component = TypeLibrary.Create<IComponent>( type );
		system.Add( component );
		return component;
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
