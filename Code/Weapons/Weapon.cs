using Sandbox;

namespace Woosh.Espionage;

public sealed class Mark23Weapon : Weapon
{
	public override void Spawn()
	{
		base.Spawn();
		Model = Model.Load( "weapons/mk23/espionage_mk23.vmdl" );
		EnableAllCollisions = true;
	}
}

public abstract partial class Weapon : AnimatedEntity, ICarriable, IPickup
{
	public AnimatedEntity Effects => IsLocalPawn ? m_Viewmodel : this;

	public StructEventDispatcher Events { get; }

	private AnimatedEntity m_Viewmodel;

	public Weapon()
	{
		Events = new StructEventDispatcher();
	}

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

	[ClientRpc]
	public void PlayClientEffects()
	{
		Particles.Create( "particles/weapons/muzzle_flash/muzzle_sup/muzzleflash_base.vpcf", Effects, "muzzle" );
	}

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

	void ICarriable.Deploying()
	{
		if ( IsLocalPawn && m_Viewmodel == null )
		{
			var view = new CompositedViewModel( Events ) { Owner = Owner, Model = Model.Load( "weapons/mk23/v_espionage_mk23.vmdl" ) };
			view.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
			view.Add( new ViewModelSwayEffect() );
			view.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
			view.Add( new ViewModelStrafeOffsetEffect() { Damping = 6, RollMultiplier = 1, AxisMultiplier = 8 } );
			view.Add( new ViewModelDeadzoneSwayEffect( new Vector2( 8, 8 ) ) { AimingOnly = true, AutoCenter = false, Damping = 8 } );
			view.Add( new ViewModelPitchOffsetEffect() );
			view.Add( new ViewModelRecoilEffect() );

			view.SetBodyGroup( "muzzle", 1 );

			m_Viewmodel = view;
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
	}
}
