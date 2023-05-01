using Sandbox;

namespace Woosh.Espionage;

public enum WeaponClientEffects
{
	Shoot,
	Reload,
	Aim
}

public struct WeaponSetup
{
	public bool IsAutomatic;
	public float Firerate;
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

		n_Setup = new WeaponSetup() { IsAutomatic = true, Firerate = 60 };
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( IsFireable( true ) )
		{
			// Shoot!
			Shoot();
		}

		if ( Input.Pressed( "reload" ) )
		{
			// Reload!
		}
	}

	public WeaponSetup Setup => n_Setup;
	[Net] private WeaponSetup n_Setup { get; set; }

	// Shoot

	[Net, Predicted, Local] protected TimeSince SinceLastShoot { get; set; }

	bool IsFireable( bool checkInput = false )
	{
		// Check if any state is running
		if ( checkInput )
		{
			// Check for input
			if ( Setup.IsAutomatic ? !Input.Down( "shoot" ) : !Input.Pressed( "shoot" ) )
				return false;
		}

		return SinceLastShoot >= 1 / Setup.Firerate;
	}

	public void Shoot()
	{
		if ( !IsFireable( false ) )
			return;

		SinceLastShoot = 0;

		if ( !Prediction.FirstTime )
			return;

		// Play Effects
		PlayClientEffects();

		// Owner, Shoot from View Model
		if ( IsLocalPawn )
		{
			var muzzle = Effects?.GetAttachment( "muzzle" ) ?? Owner.Transform;
			CmdReceivedShootRequest( NetworkIdent, muzzle.Position, muzzle.Rotation.Forward );
			return;
		}

		// No Owner, Shoot from World Model
		if ( Owner == null && Game.IsServer ) { }
	}

	[ClientRpc]
	private void PlayClientEffects()
	{
		Events.Run( new WeaponFireEvent() );
	}

	[ConCmd.Server]
	private static void CmdReceivedShootRequest( int entity, Vector3 pos, Vector3 forward )
	{
		_ = new Prop
		{
			Model = Model.Load( "models/sbox_props/watermelon/watermelon.vmdl" ),
			Position = pos + forward,
			Rotation = Rotation.LookAt( Vector3.Random.Normal ),
			Scale = 0.4f,
			PhysicsGroup = { Velocity = forward * 1000 }
		};
	}

	// Pickup

	void IPickup.OnPickup( Entity carrier )
	{
		EnableAllCollisions = false;
	}

	void IPickup.OnDrop()
	{
		EnableAllCollisions = true;

		var down = Rotation.LookAt( Owner.AimRay.Forward ).Down;
		Position = Owner.AimRay.Position + down * 24;
		Velocity += down * 12;
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
