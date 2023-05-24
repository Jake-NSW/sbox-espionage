using Woosh.Common;

namespace Woosh.Espionage;

public sealed class ViewModelHandlerComponent : ObservableEntityComponent<Pawn>, IMutateCameraSetup
{
	protected override void OnActivate()
	{
		base.OnActivate();

		Events.Register<DeployingEntity>( OnDeploying );
		Events.Register<HolsteredEntity>( OnHolstered );
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

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		m_Model?.OnPostCameraSetup( ref setup );
	}

	private CompositedViewModel m_Model;

	private CompositedViewModel OnRequestViewmodel( IObservableEntity target )
	{
		if ( !target.Events.Any<CreatedViewModel>() )
			return null;

		var view = new CompositedViewModel( target ) { Owner = Entity };
		target.Events.Run( new CreatedViewModel( view ) );
		return view;
	}

	// Deploying

	private void OnDeploying( Event<DeployingEntity> evt )
	{
		if ( UnderlyingEntity.IsLocalPawn && m_Model == null )
		{
			m_Model = OnRequestViewmodel( evt.Data.Entity as IObservableEntity );
		}
	}

	private void OnHolstered( Event<HolsteredEntity> evt )
	{
		m_Model?.Delete();
		m_Model = null;
	}
}
