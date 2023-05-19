using System;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public abstract class ObservableEntityComponent : EntityComponent
{
	[AttributeUsage( AttributeTargets.Method )]
	protected sealed class AutoAttribute : Attribute { }

	protected Dispatcher Events
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Entity as IObservableEntity)?.Events ?? throw new InvalidCastException( $"Entity is not observable - {Entity.GetType().FullName}" );
	}

	// Register

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Register<T>( StructCallback<T> evt ) where T : struct, IEventData => Events.Register( evt );
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Register<T>( Action evt ) where T : struct, IEventData => Events.Register<T>( evt );

	// Run

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Run<T>( T data = default ) where T : struct, IEventData
	{
		Events.Run( data, this );
	}

	// Unregister

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Unregister<T>( StructCallback<T> evt ) where T : struct, IEventData => Events.Unregister( evt );
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void Unregister<T>( Action evt ) where T : struct, IEventData => Events.Unregister<T>( evt );
}

public abstract class ObservableEntityComponent<T> : ObservableEntityComponent where T : class, IObservableEntity
{
	public new T Entity { get => base.Entity as T; }
	protected Entity UnderlyingEntity => base.Entity;

	public override bool CanAddToEntity( Entity entity ) => entity is T;
}
