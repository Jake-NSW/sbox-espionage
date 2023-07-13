using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelHandlerComponent : ObservableEntityComponent<Pawn>, IPostMutate<CameraSetup>, IPostMutate<InputContext>
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

	public void OnPostMutate( ref CameraSetup setup )
	{
		(m_Model as IPostMutate<CameraSetup>)?.OnPostMutate( ref setup );
	}

	public void OnPostMutate( ref InputContext setup )
	{
		(m_Model as IPostMutate<InputContext>)?.OnPostMutate( ref setup );
	}

	private AnimatedEntity m_Model;
	private AppliedViewModelEntityEffects m_Effects;

	private CompositedViewModel OnRequestViewmodel( Entity target )
	{
		var view = new CompositedViewModel( (IObservable)target ) { Owner = Entity };

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
			m_Model = OnRequestViewmodel( evt.Data.Entity );
			evt.Data.Entity.Components.Add( m_Effects = new AppliedViewModelEntityEffects( m_Model ) );
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
