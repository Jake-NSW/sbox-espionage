using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class GenericFirearmViewModelAnimator : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	protected override void OnAutoRegister()
	{
		Register<WeaponFired>( OnShoot );

		Register<DeployingEntity>( OnDeploying );
		Register<HolsteringEntity>( OnHolstering );

		Register<CheckAmmoOpen>( OnCheckAmmoOpen );
		Register<CheckAmmoClosed>( OnCheckAmmoClosed );
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
		Entity.SetAnimParameter( "bDeployed", false );
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
