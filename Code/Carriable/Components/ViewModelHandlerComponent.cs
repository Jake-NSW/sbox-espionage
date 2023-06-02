using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelHandlerComponent : ObservableEntityComponent<Pawn>, IMutate<CameraSetup>, IMutate<InputContext>
{
	protected override void OnAutoRegister()
	{
		Events.Register<DeployingEntity>( OnDeploying );
		Events.Register<HolsteredEntity>( OnHolstered );
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		// Remove Viewmodel

		m_Model?.Delete();
		m_Model = null;
	}

	public void OnPostSetup( ref CameraSetup setup )
	{
		(m_Model as IMutate<CameraSetup>)?.OnPostSetup( ref setup );
	}


	public void OnPostSetup( ref InputContext setup )
	{
		(m_Model as IMutate<InputContext>)?.OnPostSetup( ref setup );
	}

	private AnimatedEntity m_Model;

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
