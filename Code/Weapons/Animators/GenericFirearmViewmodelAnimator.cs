using Woosh.Common;

namespace Woosh.Espionage;

public sealed class GenericFirearmViewModelAnimator : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	protected override void OnActivate()
	{
		Events.Register<WeaponFired>( OnShoot );
		
		Events.Register<DeployingEntity>( OnDeploying );
		Events.Register<HolsteringEntity>( OnHolstering );
	}

	protected override void OnDeactivate()
	{
		Events.Unregister<WeaponFired>( OnShoot );
		
		Events.Unregister<DeployingEntity>( OnDeploying );
		Events.Unregister<HolsteringEntity>( OnHolstering );
	}

	private bool m_DeployedBefore;

	private void OnDeploying( Event<DeployingEntity> evt )
	{
		// Create Viewmodel
		Entity.SetAnimParameter( "bFirstDeploy", !m_DeployedBefore );
		Entity.SetAnimParameter( "bDeployed", true );
		m_DeployedBefore = true;
	}

	// Holstering

	private void OnHolstering( Event<HolsteringEntity> evt )
	{
		Entity.SetAnimParameter( "bDropped", evt.Data.Dropped );
		Entity?.SetAnimParameter( "bDeployed", false );
	}

	private void OnShoot()
	{
		Entity.SetAnimParameter( "bFire", true );
	}

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		Entity.SetAnimParameter( "fAimBlend", setup.Hands.Aim );
	}
}
