using System;
using Sandbox;

namespace Woosh.Common;

public abstract class ObservableEntityComponent : EntityComponent
{
	[AttributeUsage(AttributeTargets.Method)]
	protected sealed class AutoAttribute : Attribute{}
	
	protected Dispatcher Events => (Entity as IObservableEntity)?.Events ?? throw new InvalidCastException( "Entity is not observable" );
}

public abstract class ObservableEntityComponent<T> : ObservableEntityComponent where T : class, IObservableEntity
{
	public new T Entity { get => base.Entity as T; }
	protected Entity UnderlyingEntity => base.Entity;

	public override bool CanAddToEntity( Entity entity ) => entity is T;
}
