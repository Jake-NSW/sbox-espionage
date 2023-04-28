using Sandbox;

namespace Woosh.Espionage;

public sealed class Smg2Weapon : AnimatedEntity, ICarriable
{
	private AnimatedEntity m_View;
	public StructEventDispatcher Events { get; }

	public Smg2Weapon()
	{
		Events = new StructEventDispatcher();
	}

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "weapons/smg2/espionage_smg2.vmdl" );
	}

	void ICarriable.Deploying()
	{
		if ( IsLocalPawn && m_View == null )
		{
			var viewModel = new CompositedViewModel( Events ) { Owner = Owner, Model = Model.Load( "weapons/smg2/v_espionage_smg2.vmdl" ) };
			viewModel.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
			viewModel.Add( new ViewModelSwayEffect() );
			viewModel.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
			viewModel.Add( new ViewModelStrafeOffsetEffect() { Damping = 6, RollMultiplier = 1, AxisMultiplier = 8 } );
			viewModel.Add( new ViewModelDeadzoneSwayEffect( new Vector2( 8, 8 ) ) { AimingOnly = true, AutoCenter = false, Damping = 8 } );
			viewModel.Add( new ViewModelPitchOffsetEffect() );
			viewModel.Add( new ViewModelRecoilEffect() );

			m_View = viewModel;
		}

		if ( m_View != null )
		{
			m_View?.SetAnimParameter( "bDeployed", true );
			if ( m_View != null )
				m_View.EnableDrawing = true;
		}
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( "shoot" ) )
		{
			// Shoot
			Events.Run( new WeaponFireEvent( new Vector3( -7, 0.2f, 0.2f ) * 15, new Vector3( -8, 0.02f, 0.02f ) * 20 ) );
			m_View?.SetAnimParameter( "bFire", true );
		}
	}

	void ICarriable.Holstering( bool drop )
	{
		if ( m_View != null )
		{
			m_View.SetAnimParameter( "bDropped", drop );
			m_View.SetAnimParameter( "bDeployed", false );
		}
	}

	void ICarriable.OnHolstered()
	{
		if ( m_View != null )
			m_View.EnableDrawing = false;
	}
}
