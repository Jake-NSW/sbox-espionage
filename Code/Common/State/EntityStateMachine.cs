using Sandbox;

namespace Woosh.Common;

public sealed class EntityStateMachine<T> where T : IEntity
{
	public T Entity { get; }

	public EntityStateMachine( T attached )
	{
		Entity = attached;
	}

	public ISimulatedEntityState<T> Active => m_Active;
	private ISimulatedEntityState<T> m_Active;

	public void Simulate( IClient client )
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
				return;
			}
		}

		// Find Next thing to Simulate
		foreach ( var state in Entity.Components.GetAll<ISimulatedEntityState<T>>() )
		{
			if ( !state.TryEnter() || state == last )
			{
				continue;
			}

			// Make her the new state
			m_Active = state;
			m_Active?.OnStart();
				
			// Simulate new Target
			if ( m_Active != null && m_Active.Simulate( client ) )
			{
				m_Active.OnFinish();
				m_Active = null;
			}
		}
	}
}
