using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelHandlerComponent : ObservableEntityComponent<Pawn>, IMutate<CameraSetup>, IMutate<InputContext>
{
	protected override void OnAutoRegister()
	{
		Register<DeployingEntity>( OnDeploying );
		Register<HolsteredEntity>( OnHolstered );
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		// Remove Viewmodel

		m_Model?.Delete();
		m_Model = null;
		
		m_Effects?.Remove();
		m_Effects = null;
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
	private AppliedViewModelEntityEffects m_Effects;

	private CompositedViewModel OnRequestViewmodel( IObservable target )
	{
		var view = new CompositedViewModel( target ) { Owner = Entity };
		target.Events.Run( new CreatedViewModel( view ) );
		return view;
	}

	// Deploying

	private void OnDeploying( Event<DeployingEntity> evt )
	{
		if ( Entity.IsLocalPawn && m_Model == null )
		{
			m_Model = OnRequestViewmodel( evt.Data.Entity as IObservable );
			evt.Data.Entity.Components.Add( m_Effects = new AppliedViewModelEntityEffects( m_Model ) );
		}
	}

	private void OnHolstered( Event<HolsteredEntity> evt )
	{
		m_Model?.Delete();
		m_Model = null;
		
		m_Effects?.Remove();
		m_Effects = null;
	}

}
