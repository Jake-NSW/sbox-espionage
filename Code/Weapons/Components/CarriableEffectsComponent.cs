using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed partial class CarriableEffectsComponent : ObservableEntityComponent<ICarriable>, IEntityEffects
{
	[Net] private Model n_Viewmodel { get; set; }
	public ModelEntity Target => UnderlyingEntity.IsFirstPersonMode ? m_Model : UnderlyingEntity as ModelEntity;

	public CarriableEffectsComponent()
	{
		Game.AssertClient();
	}

	public CarriableEffectsComponent( Model viewmodel )
	{
		Game.AssertServer();
		n_Viewmodel = viewmodel;
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

		// Remove Viewmodel
		
		m_Model?.Delete();
		m_Model = null;
	}

	private AnimatedEntity m_Model;

	private AnimatedEntity OnRequestViewmodel()
	{
		var view = new CompositedViewModel( Events ) { Owner = Entity.Owner as Entity, Model = n_Viewmodel };
		view.ImportFrom<EspEffectStack>();
		return view;
	}

	// Deploying

	private void OnDeploying( in Event<DeployingEntity> evt )
	{
		if ( UnderlyingEntity.IsLocalPawn && m_Model == null )
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
