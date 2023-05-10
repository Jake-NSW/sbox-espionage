using Sandbox;

namespace Woosh.Common;

public interface IObservableEntity : IEntity
{
	Dispatcher Events { get; }
}
