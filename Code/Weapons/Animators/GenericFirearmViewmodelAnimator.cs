using Woosh.Common;

namespace Woosh.Espionage;

public sealed class GenericFirearmViewModelAnimator : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	protected override void OnActivate()
	{
		Events.Register<WeaponFired>( OnShoot );

		Events.Register<DeployingEntity>( OnDeploying );
		Events.Register<HolsteringEntity>( OnHolstering );

		Events.Register<CheckAmmoOpen>( OnCheckAmmoOpen );
		Events.Register<CheckAmmoClosed>( OnCheckAmmoClosed );
	}


	protected override void OnDeactivate()
	{
		Events.Unregister<WeaponFired>( OnShoot );

		Events.Unregister<DeployingEntity>( OnDeploying );
		Events.Unregister<HolsteringEntity>( OnHolstering );

		Events.Unregister<CheckAmmoOpen>( OnCheckAmmoOpen );
		Events.Unregister<CheckAmmoClosed>( OnCheckAmmoClosed );
	}

	// Check Ammo

	private void OnCheckAmmoClosed( Event<CheckAmmoClosed> evt )
	{
		Log.Info("Closed");
		Entity.SetAnimParameter( "bAttachmentMenu", false );
	}

	private void OnCheckAmmoOpen( Event<CheckAmmoOpen> evt )
	{
		Log.Info("Opened ");
		Entity.SetAnimParameter( "bAttachmentMenu", true );
	}

	// Deploying

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
