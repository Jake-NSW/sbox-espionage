using Sandbox;

namespace Woosh.Espionage;

public partial class CarriableHandler : EntityComponent, IActive<Entity>
{
	[Net, Local] public Entity Active { get; set; }

	// Active

	[Predicted] private Entity m_LastActive { get; set; }

	public void Simulate( IClient client )
	{
		// Constantly check if we have changed Active
		if ( m_LastActive != Active )
		{
			(m_LastActive as ICarriable)?.OnHolster( n_IsDropping );
			m_LastActive = Active;
			(m_LastActive as ICarriable)?.OnDeploying();
		}

		// Don't do anything if we're holstering
		if ( n_IsHolstering )
		{
			if ( n_SinceHolsterStart >= n_Holster )
			{
				OnHolstered();
			}

			return;
		}

		Active?.Simulate( client );

		// Wait for Deploying to Finish
		if ( n_IsDeploying )
		{
			if ( n_SinceDeployStart >= n_Deploy )
			{
				OnDeployed();
			}
		}
	}

	// Deploy

	[Net, Local] private bool n_IsDeploying { get; set; }
	[Net, Local] private Entity n_ToDeploy { get; set; }
	[Net, Local] private TimeSince n_SinceDeployStart { get; set; }
	[Net, Local] private float n_Deploy { get; set; }

	public void Deploy( Entity entity, float deployTime, float holsterTime )
	{
		if ( !Game.IsServer )
			return;

		if ( !entity.IsValid() || Active == entity )
			return;

		if ( !Entity.IsAuthority || entity.Owner != Entity )
			return;

		n_ToDeploy = entity;
		n_Deploy = deployTime;
		m_StoredHolster = holsterTime;

		Log.Info( $"Preparing deploy - {n_ToDeploy}" );

		if ( Active != null && !n_IsDeploying )
		{
			Holster( false );
			return;
		}

		if ( Active == null )
		{
			OnReadyToDeploy();
		}
	}

	private float m_StoredHolster;

	private void OnReadyToDeploy()
	{
		if ( !Game.IsServer )
			return;

		// Call Active Start
		Active = n_ToDeploy;
		n_ToDeploy = null;

		n_SinceDeployStart = 0;
		n_IsDeploying = true;

		Log.Info( $"Ready to deploy - {Active}" );
	}

	private void OnDeployed()
	{
		if ( !Game.IsServer )
			return;

		n_IsDeploying = false;
		n_Holster = m_StoredHolster;

		Log.Info( $"Deployed - {Active} / {n_SinceDeployStart}" );
		if ( n_ToDeploy != null )
		{
			Holster( false );
		}
	}

	// Holster

	[Net, Local] private bool n_IsDropping { get; set; }
	[Net, Local] private bool n_IsHolstering { get; set; }
	[Net, Local] private TimeSince n_SinceHolsterStart { get; set; }
	[Net, Local] private float n_Holster { get; set; }

	public void Holster( bool dropping )
	{
		if ( !Game.IsServer )
			return;

		Log.Info( $"Holstering - {Active}" );

		Active = null;
		n_IsDropping = dropping;
		n_IsHolstering = true;
		n_SinceHolsterStart = 0;
	}

	private void OnHolstered()
	{
		if ( !Game.IsServer )
			return;

		Log.Info( $"Finished Holstering - {n_SinceHolsterStart}" );

		n_IsHolstering = false;
		n_IsDropping = false;

		if ( n_ToDeploy != null )
		{
			OnReadyToDeploy();
		}
	}
}
