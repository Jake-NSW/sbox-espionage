using Sandbox;

namespace Woosh.Espionage;

[Library, Title("Hands"), Icon("pan_tool")]
public sealed partial class PlayerHands : AnimatedEntity, ICarriable
{
	// Attack

	public void OnSwingStart() { }

	public void OnSwingHit( in TraceResult result ) { }

	[Net, Local, Predicted] private TimeSince n_SinceAttack { get; set; }

	private void Attack()
	{
		n_SinceAttack = 0;
		// Raycast and what not...
	}

	// ICarriable

	public DrawTime Draw => new DrawTime( 1, 0.5f );

	public AnimatedEntity Effects => IsLocalPawn ? m_Viewmodel : this;
	private AnimatedEntity m_Viewmodel;

	public void Deploying()
	{
		if ( IsLocalPawn && m_Viewmodel == null )
		{
			var view = new CompositedViewModel( null ) { Owner = Owner, Model = Model.Load( "weapons/hands/v_espionage_hands.vmdl" ) };
			view.ImportFrom<EspEffectStack>();
			m_Viewmodel = view;
		}

		// Create Viewmodel
		Effects.SetAnimParameter( "bDeployed", true );
		Effects.EnableDrawing = true;
	}

	public void Holstering( bool drop )
	{
		Effects?.SetAnimParameter( "bDeployed", false );
	}

	public void OnHolstered()
	{
		if ( Effects != null )
			Effects.EnableDrawing = false;

		m_Viewmodel?.Delete();
		m_Viewmodel = null;
	}
}
