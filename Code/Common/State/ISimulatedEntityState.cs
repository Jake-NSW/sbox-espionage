using Sandbox;

namespace Woosh.Common;

public interface ISimulatedEntityState<T> : IComponent where T : IEntity
{
	bool TryEnter();

	bool Simulate( IClient cl );

	void OnStart();
	void OnFinish();
}
