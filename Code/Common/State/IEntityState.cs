using Sandbox;

namespace Woosh.Espionage;

/// <summary>
/// A State an entity can be in that is injected from a component.
/// <typeparam name="T"> The target entity type that this state can be attached too. </typeparam>
/// </summary>
public interface IEntityState<T> : IComponent where T : IEntity
{
	/// <summary>
	/// Called when the state is attempted to be entered. This is called by the <see cref="EntityStateMachine{T}"/> when the entity is
	/// ready to go into another state if it is not already in one. 
	/// <returns> If we are able to enter this state. </returns>
	/// </summary>
	bool TryEnter();

	/// <summary>
	/// The simulation loop for this state. This is called by the <see cref="EntityStateMachine{T}"/> when the entity is in this state.
	/// <param name="cl"> The client that is simulating this state. </param>
	/// <returns> <b>True</b> if we are finished with this state. </returns>
	/// </summary>
	bool Simulate( IClient cl );

	/// <summary>
	/// A callback from the <see cref="EntityStateMachine{T}"/> when this state is entered. This is called after <see cref="TryEnter"/>
	/// was called and returned true.
	/// </summary>
	void OnStart() { }

	/// <summary>
	/// A callback from the <see cref="EntityStateMachine{T}"/> when this state is finished. This is called after <see cref="Simulate"/>
	/// has returned true.
	/// </summary>
	void OnFinish() { }
}
