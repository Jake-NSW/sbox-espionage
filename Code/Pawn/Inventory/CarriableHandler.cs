using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public delegate void EntityComponentCallback( Entity from );

public partial class CarriableHandler : ObservableEntityComponent<Pawn>, IActive<Entity>, IActive<ICarriable>, ISingletonComponent, ISimulated
{
	[Net, Local] public Entity Active { get; set; }
	ICarriable IActive<ICarriable>.Active => Active as ICarriable;

	// Active

	[Net, Local] private Entity n_RealActive { get; set; }
	[Predicted] private Entity m_LastActive { get; set; }

	public void Simulate( IClient client )
	{
		// Constantly check if we have changed Active
		if ( m_LastActive != n_RealActive )
		{
			SimulatedHolstering();
			m_LastActive = n_RealActive;
			SimulatedDeploying();
		}

		// Wait for Deploying to Finish
		if ( n_IsDeploying )
		{
			if ( n_SinceDeployStart >= n_Deploy )
			{
				OnDeployed();
			}
		}

		// Don't do anything if we're holstering
		if ( n_IsHolstering )
		{
			if ( n_SinceHolsterStart >= n_Holster )
			{
				OnHolstered();
			}
		}

		if ( !n_IsDeploying && !n_IsHolstering )
			Active?.Simulate( client );
	}

	private void SimulatedHolstering()
	{
		(m_LastActive as ICarriable)?.Holstering( n_IsDropping );
		Run( new HolsteringEntity( m_LastActive, n_IsDropping ) );
		
		// We only want to propagate to the active entity, so do this for now.
		(m_LastActive as IObservableEntity)?.Events.Run( new HolsteringEntity( m_LastActive, n_IsDropping ), from: this );
	}

	private void SimulatedDeploying()
	{
		(m_LastActive as ICarriable)?.Deploying();
		Run( new DeployingEntity( m_LastActive ) );
		
		// We only want to propagate to the active entity, so do this for now.
		(m_LastActive as IObservableEntity)?.Events.Run( new DeployingEntity( m_LastActive ), from: this );
	}

	// Deploy

	[Net, Local] private bool n_IsDeploying { get; set; }
	[Net, Local] private Entity n_ToDeploy { get; set; }
	[Net, Local] private TimeSince n_SinceDeployStart { get; set; }
	[Net, Local] private float n_Deploy { get; set; }

	private EntityComponentCallback m_OnDeployed;

	public void Deploy( Entity entity, DrawTime? timings = null, EntityComponentCallback onDeployed = null )
	{
		// Only allow the server to deploy...
		Game.AssertServer();

		// Check if we have a valid entity
		if ( !entity.IsValid() || Active == entity )
			return;

		// Check if we actually own the entity
		if ( !Entity.IsAuthority || entity.Owner != Entity )
			return;

		// Check if we can actually hold this
		if ( entity is ICarriable { Deployable: false } )
			return;

		// Save for when are ready to deploy
		m_OnDeployed = onDeployed;
		n_ToDeploy = entity;
		m_StoredDelay = timings ?? (entity as ICarriable)?.Draw ?? default;

		if ( n_IsDeploying || n_IsHolstering )
		{
			return;
		}

		if ( Active != null )
		{
			Holster( false );
			return;
		}

		if ( Active == null )
		{
			OnReadyToDeploy();
		}
	}

	private DrawTime m_StoredDelay;

	private void OnReadyToDeploy()
	{
		if ( n_ToDeploy == null || !Game.IsServer )
			return;

		// Call Active Start
		n_RealActive = n_ToDeploy;
		Active = n_RealActive;
		n_ToDeploy = null;

		n_Deploy = m_StoredDelay.Deploy;
		n_Holster = m_StoredDelay.Holster;

		n_IsDeploying = true;
		n_SinceDeployStart = 0;
	}

	private void OnDeployed()
	{
		n_IsDeploying = false;

		m_OnDeployed?.Invoke( Active );

		Run( new DeployedEntity( Active ) );
		(Active as IObservableEntity)?.Events.Run( new DeployedEntity( Active ), from: this );

		m_OnDeployed = null;

		if ( n_ToDeploy != null && Game.IsServer )
		{
			Holster( false );
		}
	}

	// Holster

	[Net, Local] private bool n_IsDropping { get; set; }
	[Net, Local] private bool n_IsHolstering { get; set; }
	[Net, Local] private TimeSince n_SinceHolsterStart { get; set; }
	[Net, Local] private float n_Holster { get; set; }

	private EntityComponentCallback m_OnHolstered;

	public void Holster( bool dropping, EntityComponentCallback onHolstered = null )
	{
		Game.AssertServer();

		if ( Active is ICarriable { Holsterable: false } )
		{
			// Deploy Fallback
			n_ToDeploy = null;
			return;
		}

		m_OnHolstered = onHolstered;
		n_RealActive = null;
		n_IsDropping = dropping;
		n_IsHolstering = true;
		n_SinceHolsterStart = 0;
	}

	private void OnHolstered()
	{
		n_IsHolstering = false;

		(Active as ICarriable)?.OnHolstered();
		Run( new HolsteredEntity( Active ) );
		(Active as IObservableEntity)?.Events.Run( new HolsteredEntity( Active ), from: this );

		n_IsDropping = false;

		m_OnHolstered?.Invoke( Entity );
		m_OnHolstered = null;

		Active = null;

		if ( n_ToDeploy != null )
		{
			OnReadyToDeploy();
		}
	}
}
