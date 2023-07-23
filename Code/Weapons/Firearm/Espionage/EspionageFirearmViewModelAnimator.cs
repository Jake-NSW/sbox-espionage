using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class EspionageFirearmViewModelAnimator : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
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
		Entity.SetAnimParameter( "bDropped", evt.Signal.Dropped );
		Entity.SetAnimParameter( "bDeployed", false );
	}

	[Listen]
	private void OnShoot( Event<FirearmFired> evt )
	{
		Entity.SetAnimParameter( "bFire", true );
	}

	[Listen]
	private void OnReload( Event<ReloadStarted> evt )
	{
		Entity.SetAnimParameter( "bReload", true );
	}
	
	public void OnMutate( ref CameraSetup setup )
	{
		Entity.SetAnimParameter( "fAimBlend", setup.Hands.Aim );
	}
}
