using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

/// <summary>
/// An Entity State Machine is a component based state machine that can be used to simulate an entity state by state. This is useful
/// for overriding the default entity logic to something defined in a component. Make sure to call <see cref="Simulate"/> in the
/// entities simulate method, or else it won't work.
/// <typeparam name="T"> The target entity type that will be stateful </typeparam>
/// </summary>
public sealed class EntityStateMachine<T> where T : IEntity
{
	private T m_Entity;
	private IEntityState<T> m_Active;

	public EntityStateMachine( T attached )
	{
		m_Entity = attached;
		m_Active = null;
	}

	/// <summary>
	/// Is the entity currently in a state? Checks if <see cref="Active"/> is null. Meant to provide a more readable way to check if
	/// the entity is in a state, as "Machine.Active != null" is a bit vague..
	/// </summary>
	public bool InState
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get => Active != null;
	}

	/// <summary>
	/// The current active state. This is the state that is currently being simulated. If this is null, then the entity is not in a
	/// state. This is set by <see cref="Simulate"/> and is not meant to be set manually. The <see cref="IEntityState{T}"/>
	/// is responsible for activating itself by returning true from <see cref="IEntityState{T}.TryEnter"/>.
	/// </summary>
	public IEntityState<T> Active => m_Active;

	/// <summary>
	/// Simulates the entity state machine. This will call <see cref="IEntityState{T}.Simulate"/> on the current active and
	/// will attempt to find a new active state if the current active state is null. This is meant to be called in the entities own
	/// simulate method.
	/// <returns> If we are currently in a state or not. </returns>
	/// </summary>
	public bool Simulate( IClient client )
	{
		var last = m_Active;

		// Currently Active, only care about it
		if ( m_Active != null )
		{
			if ( m_Active.Simulate( client ) )
			{
				m_Active?.OnFinish();
				m_Active = null;
			}
			else
			{
				return false;
			}
		}

		// Find Next thing to Simulate
		foreach ( var state in m_Entity.Components.GetAll<IEntityState<T>>() )
		{
			if ( !state.TryEnter() || state == last )
			{
				continue;
			}

			// Make her the new state
			m_Active = state;
			m_Active?.OnStart();
		}

		return true;
	}
}
