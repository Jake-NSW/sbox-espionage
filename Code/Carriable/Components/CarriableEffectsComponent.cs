using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class CarriableEffectsComponent : ObservableEntityComponent<ICarriable>, IEntityEffects
{
	public ModelEntity Target => UnderlyingEntity.IsFirstPersonMode ? m_Model : UnderlyingEntity as ModelEntity;

	protected override void OnActivate()
	{
		base.OnActivate();

		Events.Register<DeployingEntity>( OnDeploying );
		Events.Register<HolsteredEntity>( OnHolstered );
		Events.Register<HolsteringEntity>( OnHolstering );
	}

	protected override void OnDeactivate()
	{
		base.OnActivate();

		Events.Unregister<DeployingEntity>( OnDeploying );
		Events.Unregister<HolsteredEntity>( OnHolstered );

		// Remove Viewmodel

		m_Model?.Delete();
		m_Model = null;
	}

	private AnimatedEntity m_Model;

	private AnimatedEntity OnRequestViewmodel()
	{
		var view = new CompositedViewModel( Entity ) { Owner = Entity.Owner as Entity };
		Events.Run( new CreateViewModel( view ) );
		return view;
	}

	// Deploying

	private bool m_DeployedBefore;

	private void OnDeploying( Event<DeployingEntity> evt )
	{
		if ( UnderlyingEntity.IsLocalPawn && m_Model == null )
			m_Model = OnRequestViewmodel();

		// Create Viewmodel
		m_Model?.SetAnimParameter( "bFirstDeploy", !m_DeployedBefore );
		m_Model?.SetAnimParameter( "bDeployed", true );

		m_DeployedBefore = true;
	}

	// Holstering

	private void OnHolstering( Event<HolsteringEntity> evt )
	{
		m_Model?.SetAnimParameter( "bDropped", evt.Data.Dropped );
		m_Model?.SetAnimParameter( "bDeployed", false );
	}

	private void OnHolstered( Event<HolsteredEntity> evt )
	{
		m_Model?.Delete();
		m_Model = null;
	}
}
