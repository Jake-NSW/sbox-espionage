using Sandbox;

namespace Woosh.Common;

public abstract class ObservableEntityComponent<T> : EntityComponent<T> where T : Entity, IObservableEntity
{
	protected Dispatcher Events => Entity.Events;
}
