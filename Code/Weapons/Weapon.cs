using Sandbox;

namespace Woosh.Espionage;

public enum WeaponClientEffects
{
	Shoot,
	Reload,
	Aim
}

public abstract partial class Weapon : AnimatedEntity, ICarriable, IPickup
{
	public StructEventDispatcher Events { get; }

	public Weapon()
	{
		Events = new StructEventDispatcher();
	}

	public override void Spawn()
	{
		base.Spawn();

		PhysicsEnabled = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( "shoot" ) )
		{
			// Shoot!
		}

		if ( Input.Pressed( "reload" ) )
		{
			// Reload!
		}
	}

	public virtual SoundBank<WeaponClientEffects> Sounds { get; }

	[ClientRpc]
	public void PlayClientEffects( WeaponClientEffects effects )
	{
		Particles.Create( "particles/weapons/muzzle_flash/muzzleflash_base.vpcf", Effects, "muzzle" );
		Sounds.Play( effects );
	}

	// Pickup

	void IPickup.OnPickup( Entity carrier )
	{
		EnableAllCollisions = false;
	}

	void IPickup.OnDrop()
	{
		EnableAllCollisions = true;
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
		Effects.SetAnimParameter( "bDropped", drop );
		Effects.SetAnimParameter( "bDeployed", false );
	}

	void ICarriable.OnHolstered()
	{
		if ( Effects != null )
			Effects.EnableDrawing = false;

		m_Viewmodel?.Delete();
		m_Viewmodel = null;
	}
}
