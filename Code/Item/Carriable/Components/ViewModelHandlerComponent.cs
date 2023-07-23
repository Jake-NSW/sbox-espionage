using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelHandlerComponent : ObservableEntityComponent<Pawn>, IMutate<CameraSetup>, IMutate<InputContext>
{
	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		// Remove Viewmodel

		m_ViewModel?.Delete();
		m_ViewModel = null;

		m_Effects?.Remove();
		m_Effects = null;
	}

	public void OnMutate( ref CameraSetup setup )
	{
		(m_ViewModel as IMutate<CameraSetup>)?.OnMutate( ref setup );
	}

	public void OnMutate( ref InputContext setup )
	{
		(m_ViewModel as IMutate<InputContext>)?.OnMutate( ref setup );
	}

	private ViewModel m_ViewModel;
	private AppliedViewModelEntityEffects m_Effects;

	private ViewModel OnRequestViewmodel( Entity target )
	{
		var view = new ViewModel( (IObservable)target )
		{
			Owner = Entity
		};

		((IObservable)target).Events.Run( new CreatedViewModel( view, target ) );
		Events.Run( new CreatedViewModel( view, target ) );

		return view;
	}

	// Deploying

	[Listen]
	private void OnDeploying( Event<DeployingEntity> evt )
	{
		if ( Entity.IsLocalPawn && m_ViewModel == null )
		{
			m_ViewModel = OnRequestViewmodel( evt.Signal.Entity );
			evt.Signal.Entity.Components.Add( m_Effects = new AppliedViewModelEntityEffects( m_ViewModel ) );
		}
	}

	[Listen]
	private void OnHolstered( Event<HolsteredEntity> evt )
	{
		m_ViewModel?.Delete();
		m_ViewModel = null;

		m_Effects?.Remove();
		m_Effects = null;
	}

}
