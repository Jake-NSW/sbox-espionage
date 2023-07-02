using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Signals;
using IComponent = Sandbox.IComponent;

namespace Woosh.Common;

public static class ComponentUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Each<TComponent, TValue>( this IComponentSystem system, TValue value, Action<TValue, TComponent> loop )
	{
		foreach ( var component in system.All() )
		{
			if ( component is TComponent cast )
				loop.Invoke( value, cast );
		}
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Each<TComponent, TValue>( this IComponentSystem system, ref TValue value, RefStructInputAction<TValue, TComponent> loop )
	{
		foreach ( var component in system.All() )
		{
			if ( component is TComponent cast )
				loop.Invoke( ref value, cast );
		}
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Each<TComponent>( this IComponentSystem system, Action<TComponent> loop )
	{
		foreach ( var component in system.All() )
		{
			if ( component is TComponent cast )
				loop.Invoke( cast );
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

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Replace<TLast, TNew>( this IComponentSystem system, TNew comp ) where TLast : IComponent where TNew : class, IComponent
	{
		system.RemoveAny<TLast>();
		system.Add( comp );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static TNew Replace<TLast, TNew>( this IComponentSystem system ) where TLast : IComponent where TNew : class, IComponent, new()
	{
		var comp = new TNew();
		system.Replace<TLast, TNew>( comp );
		return comp;
	}
}
