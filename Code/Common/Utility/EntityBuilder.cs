using System;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public readonly struct EntityBuilder<T> where T : Entity
{
	public T Entity { get; }

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public EntityBuilder( T entity )
	{
		Entity = entity;
	}

	public EntityBuilder<TNew> WithChild<TNew>( TNew value, bool boneMerge = false ) where TNew : Entity
	{
		value.SetParent( Entity, true );
		return new EntityBuilder<TNew>( value );
	}

	// Components

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public EntityBuilder<T> WithTransform( Vector3? position = default, Rotation? rotation = default )
	{
		Entity.Position = position ?? Entity.Position;
		Entity.Rotation = rotation ?? Entity.Rotation;
		return this;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public EntityBuilder<T> WithTransform( Transform transform )
	{
		Entity.Position = transform.Position;
		Entity.Rotation = transform.Rotation;
		return this;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public EntityBuilder<T> WithComponent<TComponent>( TComponent comp ) where TComponent : class, IComponent
	{
		Entity.Components.Add( comp );
		return this;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public EntityBuilder<T> WithComponent<TComponent>() where TComponent : class, IComponent, new()
	{
		Entity.Components.Create<TComponent>();
		return this;
	}

	public EntityBuilder<T> WithoutAnyComponent<TComponent>() where TComponent : class, IComponent
	{
		Entity.Components.RemoveAny<TComponent>();
		return this;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public EntityBuilder<T> WithAspect<TAspect>( TAspect aspect = default ) where TAspect : struct, IEntityAspect<T>
	{
		Entity.Import( aspect );
		return this;
	}

	public EntityBuilder<T> MutateComponent<TComponent>( Action<TComponent> mutator ) where TComponent : class, IComponent
	{
		var comp = Entity.Components.Get<TComponent>();
		mutator( comp );
		return this;
	}
}

public static class EntityBuilderUtility
{
	// Physics

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> WithPhysics<T>( this EntityBuilder<T> builder, PhysicsMotionType type, bool startAsleep = false ) where T : ModelEntity
	{
		builder.Entity.SetupPhysicsFromModel( type, startAsleep );
		return builder;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> WithPhysics<T>( this EntityBuilder<T> builder, PhysicsMotionType type, Vector3 center, float radius ) where T : ModelEntity
	{
		builder.Entity.SetupPhysicsFromSphere( type, center, radius );
		return builder;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> WithPhysics<T>( this EntityBuilder<T> builder, PhysicsMotionType type, Vector3 min, Vector3 max, bool axisAligned = true ) where T : ModelEntity
	{
		if ( axisAligned )
			builder.Entity.SetupPhysicsFromAABB( type, min, max );
		else
			builder.Entity.SetupPhysicsFromOBB( type, min, max );

		return builder;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> WithPhysics<T>( this EntityBuilder<T> builder, PhysicsMotionType type, Capsule capsule, bool flat = false, bool oriented = false ) where T : ModelEntity
	{
		if ( flat )
		{
			builder.Entity.SetupPhysicsFromCylinder( type, capsule );
			return builder;
		}

		if ( oriented )
			builder.Entity.SetupPhysicsFromOrientedCapsule( type, capsule );
		else
			builder.Entity.SetupPhysicsFromCapsule( type, capsule );

		return builder;
	}

	// Model

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> WithMaterialGroup<T>( this EntityBuilder<T> builder, int value ) where T : ModelEntity
	{
		builder.Entity.SetMaterialGroup( value );
		return builder;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> WithMaterialGroup<T>( this EntityBuilder<T> builder, string value ) where T : ModelEntity
	{
		builder.Entity.SetMaterialGroup( value );
		return builder;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> WithBodyGroup<T>( this EntityBuilder<T> builder, int index, int value ) where T : ModelEntity
	{
		builder.Entity.SetBodyGroup( index, value );
		return builder;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> WithBodyGroup<T>( this EntityBuilder<T> builder, string name, int value ) where T : ModelEntity
	{
		builder.Entity.SetBodyGroup( name, value );
		return builder;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> WithModel<T>( this EntityBuilder<T> builder, Model model ) where T : ModelEntity
	{
		builder.Entity.Model = model;
		return builder;
	}

	// Animator

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static EntityBuilder<T> ApplyAnimParam<T, TValue>( this EntityBuilder<T> builder, string name, TValue value ) where T : AnimatedEntity where TValue : struct
	{
		// Probably bad for performance.. oh well

		switch ( value )
		{
			case bool casted :
				builder.Entity.SetAnimParameter( name, casted );
				break;
			case float casted :
				builder.Entity.SetAnimParameter( name, casted );
				break;
			case Vector3 casted :
				builder.Entity.SetAnimParameter( name, casted );
				break;
			case Rotation casted :
				builder.Entity.SetAnimParameter( name, casted );
				break;
			case int casted :
				builder.Entity.SetAnimParameter( name, casted );
				break;
			case Transform casted :
				builder.Entity.SetAnimParameter( name, casted );
				break;
			default :
				throw new ArgumentException();
		}

		return builder;
	}

}
