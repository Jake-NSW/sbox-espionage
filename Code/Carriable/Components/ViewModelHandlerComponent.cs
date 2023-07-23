using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelHandlerComponent : ObservableEntityComponent<Pawn>, IMutate<CameraSetup>, IMutate<InputContext>
{
	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		// Remove Viewmodel

		m_Model?.Delete();
		m_Model = null;

		m_Effects?.Remove();
		m_Effects = null;
	}

	public void OnMutate( ref CameraSetup setup )
	{
		(m_Model as IMutate<CameraSetup>)?.OnMutate( ref setup );
	}

	public void OnMutate( ref InputContext setup )
	{
		(m_Model as IMutate<InputContext>)?.OnMutate( ref setup );
	}

	private AnimatedEntity m_Model;
	private AppliedViewModelEntityEffects m_Effects;

	private ViewModel OnRequestViewmodel( Entity target )
	{
		var view = new ViewModel( (IObservable)target ) { Owner = Entity };

		((IObservable)target).Events.Run( new CreatedViewModel( view, target ) );
		Events.Run( new CreatedViewModel( view, target ) );

		return view;
	}
	
	// Deploying

	[Listen]
	private void OnDeploying( Event<DeployingEntity> evt )
	{
		if ( Entity.IsLocalPawn && m_Model == null )
		{
			m_Model = OnRequestViewmodel( evt.Signal.Entity );
			evt.Signal.Entity.Components.Add( m_Effects = new AppliedViewModelEntityEffects( m_Model ) );
		}
	}

	[Listen]
	private void OnHolstered( Event<HolsteredEntity> evt )
	{
		m_Model?.Delete();
		m_Model = null;

		m_Effects?.Remove();
		m_Effects = null;
	}

}
