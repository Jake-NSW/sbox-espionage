using Sandbox;

namespace Woosh.Espionage;

// Aimming might be a good idea as a component (Maybe?)
//
// Implement a basic state machine for weapon? (Maybe?)

public abstract partial class Weapon : AnimatedEntity, ICarriable, IPickup
{
	public StructEventDispatcher Events { get; }

	public Weapon()
	{
		Events = new StructEventDispatcher();
	}

	private float m_LastAim;

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( "shoot" ) )
		{
			// shoot!!
			Events.Run( new WeaponFireEvent( new Vector3( -7, 0.2f, 0.2f ) * 15, new Vector3( -8, 0.02f, 0.02f ) * 20 ) );
			Effects?.SetAnimParameter( "bFire", true );

			if ( Game.IsServer && Prediction.FirstTime )
			{
				PlayClientEffects();
			}
		}
	}

	[Event.Client.Frame]
	public void aim()
	{
		var aiming = Input.Down( "aim" );
		m_LastAim = m_LastAim.LerpTo( aiming ? 1 : 0, 8 * Time.Delta );
		Effects?.SetAnimParameter( "fAimBlend", m_LastAim );
	}

	[ClientRpc]
	public void PlayClientEffects()
	{
		Particles.Create( "particles/weapons/muzzle_flash/muzzleflash_base.vpcf", Effects, "muzzle" );
	}

	// Pickup

	void IPickup.OnPickup( Entity carrier )
	{
		EnableHideInFirstPerson = true;
		EnableAllCollisions = false;
	}

	void IPickup.OnDrop()
	{
		EnableAllCollisions = true;
		EnableHideInFirstPerson = false;
	}

	// Carriable

	public AnimatedEntity Effects => IsLocalPawn ? m_Viewmodel : this;
	private AnimatedEntity m_Viewmodel;

	protected virtual AnimatedEntity OnRequestViewmodel()
	{
		var view = new CompositedViewModel( Events ) { Owner = Owner, Model = Model.Load( "weapons/mk23/v_espionage_mk23.vmdl" ) };
		view.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
		view.Add( new ViewModelSwayEffect() );
		view.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
		view.Add( new ViewModelStrafeOffsetEffect() { Damping = 6, RollMultiplier = 1, AxisMultiplier = 8 } );
		view.Add( new ViewModelDeadzoneSwayEffect( new Vector2( 8, 8 ) ) { AimingOnly = true, AutoCenter = false, Damping = 8 } );
		view.Add( new ViewModelPitchOffsetEffect() );
		view.Add( new ViewModelRecoilEffect() );
		return view;
	}

	public virtual DrawTime Draw => new DrawTime( 1, 1 );

	void ICarriable.Deploying()
	{
		if ( IsLocalPawn && m_Viewmodel == null )
		{
			m_Viewmodel = OnRequestViewmodel();
		}

		// Create Viewmodel
		Effects.SetAnimParameter( "bDeployed", true );
		Effects.EnableDrawing = true;
	}

	void ICarriable.Holstering( bool drop )
	{
		Effects.SetAnimParameter( "bDrop", drop );
		Effects.SetAnimParameter( "bDeployed", false );
	}

	void ICarriable.OnHolstered()
	{
		Effects.EnableDrawing = false;

		m_Viewmodel?.Delete();
		m_Viewmodel = null;
	}
}
