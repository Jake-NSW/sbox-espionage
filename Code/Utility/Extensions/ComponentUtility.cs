using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

public static class ComponentUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Run<T>( this IComponentSystem system, Action<T> cb )
	{
		foreach ( var component in system.All().OfType<T>() )
		{
			cb.Invoke( component );
		}
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Run<T, Arg1>( this IComponentSystem system, Arg1 arg1, Action<T, Arg1> cb )
	{
		foreach ( var component in system.All().OfType<T>() )
		{
			cb.Invoke( component, arg1 );
		}
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static IEnumerable<IComponent> All( this IComponentSystem system )
	{
		return system.GetAll<IComponent>();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T UnrestrictedGet<T>( this IComponentSystem components )
	{
		return (T)components.GetAll<IComponent>().FirstOrDefault( e => e is T );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool Has<T>( this IComponentSystem components ) where T : IComponent
	{
		return components.Get<T>() != null;
	}
}
