using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class GenericFirearmViewModelAnimator : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	// Check Ammo

	[Listen]
	private void OnCheckAmmoClosed( Event<CheckAmmoClosed> evt )
	{
		Log.Info( "Check Ammo Closed" );
		Entity.SetAnimParameter( "bAttachmentMenu", false );
	}

	[Listen]
	private void OnCheckAmmoOpen( Event<CheckAmmoOpen> evt )
	{
		Log.Info( "Check Ammo Open" );
		Entity.SetAnimParameter( "bAttachmentMenu", true );
	}

	// Deploying

	private bool m_DeployedBefore;

	[Listen]
	private void OnDeploying( Event<DeployingEntity> evt )
	{
		// Create Viewmodel
		Entity.SetAnimParameter( "bFirstDeploy", !m_DeployedBefore );
		Entity.SetAnimParameter( "bDeployed", true );
		m_DeployedBefore = true;
	}

	// Holstering

	[Listen]
	private void OnHolstering( Event<HolsteringEntity> evt )
	{
		Entity.SetAnimParameter( "bDropped", evt.Data.Dropped );
		Entity.SetAnimParameter( "bDeployed", false );
	}

	[Listen]
	private void OnShoot( Event<WeaponFired> evt )
	{
		Entity.SetAnimParameter( "bFire", true );
	}

	public void OnPostSetup( ref CameraSetup setup )
	{
		Entity.SetAnimParameter( "fAimBlend", setup.Hands.Aim );
	}
}
