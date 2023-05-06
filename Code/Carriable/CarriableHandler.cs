using Sandbox;

namespace Woosh.Espionage;

public delegate void EntityComponentCallback( Entity from );

public partial class CarriableHandler : EntityComponent, IActive<Entity>, IActive<ICarriable>, ISingletonComponent
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
			(m_LastActive as ICarriable)?.Holstering( n_IsDropping );
			m_LastActive = n_RealActive;
			(m_LastActive as ICarriable)?.Deploying();

			if ( n_Holster.AlmostEqual( 0 ) )
			{
				// Just Holster straight away if we have no holster time
				OnHolstered();
			}
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
				Log.Info("Holstering");
				OnHolstered();
			}
		}

		if ( !n_IsDeploying && !n_IsHolstering )
			Active?.Simulate( client );
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
		
		m_OnDeployed?.Invoke(Entity);
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
		n_IsDropping = false;

		(Active as ICarriable)?.OnHolstered();
		Active = null;

		m_OnHolstered?.Invoke(Entity);
		m_OnHolstered = null;

		if ( n_ToDeploy != null )
		{
			OnReadyToDeploy();
		}
	}
}
