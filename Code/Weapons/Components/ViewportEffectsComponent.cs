using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class ViewportEffectsComponent : ObservableEntityComponent<Firearm>, IEntityEffects
{
	private readonly string m_Viewmodel;
	public ModelEntity Target => Entity.IsFirstPersonMode ? m_Model : Entity;

	public ViewportEffectsComponent( string viewmodel )
	{
		Game.AssertClient();
		m_Viewmodel = viewmodel;
	}

	protected override void OnActivate()
	{
		base.OnActivate();

		Events.Register<DeployingEntity>( OnDeploying );
		Events.Register<HolsteredEntity>( OnHolstered );
		Events.Register<HolsteringEntity>( OnHolstering );
	}

	protected override void OnDeactivate()
	{
		base.OnActivate();

		Events.Unregister<DeployingEntity>( OnDeploying );
		Events.Unregister<HolsteredEntity>( OnHolstered );
	}

	private AnimatedEntity m_Model;

	private AnimatedEntity OnRequestViewmodel()
	{
		var view = new CompositedViewModel( Events ) { Owner = Entity.Owner, Model = Model.Load( m_Viewmodel ) };
		view.ImportFrom<EspEffectStack>();
		return view;
	}

	// Deploying

	private void OnDeploying( in Event<DeployingEntity> evt )
	{
		if ( Entity.IsLocalPawn && m_Model == null )
			m_Model = OnRequestViewmodel();

		// Create Viewmodel
		m_Model?.SetAnimParameter( "bDeployed", true );
	}

	// Holstering

	private void OnHolstering( in Event<HolsteringEntity> evt )
	{
		m_Model?.SetAnimParameter( "bDropped", evt.Data.Dropped );
		m_Model?.SetAnimParameter( "bDeployed", false );
	}

	private void OnHolstered( in Event<HolsteredEntity> evt )
	{
		m_Model?.Delete();
		m_Model = null;
	}
}
