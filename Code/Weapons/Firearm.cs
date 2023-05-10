using System.Collections.Generic;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public enum WeaponSound
{
	Shoot,
	Reload
}

public struct FirearmSetup
{
	public bool IsAutomatic;
	public float RateOfFire; // RPM
	public float Control;
	public float Mobility;
	public float Range;
	public float Accuracy;
}

public abstract partial class Firearm : ObservableAnimatedEntity, ICarriable, IPickup
{
	public override void Spawn()
	{
		base.Spawn();

		PhysicsEnabled = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		n_Setup = new FirearmSetup { IsAutomatic = true, RateOfFire = 900 };
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( IsFireable( true ) )
		{
			// Shoot!
			Shoot();
		}
	}

	public FirearmSetup Setup => n_Setup;
	[Net] private FirearmSetup n_Setup { get; set; }

	// Shoot

	public virtual SoundBank<WeaponSound> Sounds { get; } = new SoundBank<WeaponSound>( new Dictionary<WeaponSound, string> { { WeaponSound.Shoot, "smg2_firing_suppressed_sound" } } );

	[Net, Predicted, Local] private TimeSince n_SinceLastShot { get; set; }

	bool IsFireable( bool checkInput = false )
	{
		// Check if any state is running
		if ( checkInput )
		{
			// Check for input
			if ( Setup.IsAutomatic ? !Input.Down( "shoot" ) : !Input.Pressed( "shoot" ) )
				return false;
		}
		
		return n_SinceLastShot >= 60 / Setup.RateOfFire;
	}

	public void Shoot()
	{
		if ( !IsFireable() )
			return;

		n_SinceLastShot = 0;

		if ( !Prediction.FirstTime )
			return;

		Events.Run( new WeaponFireEvent( new Vector3( -3, 0.2f, 0.2f ) * 35, new Vector3( -1, 0.2f, 0.2f ) * 35 ) );

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
		if ( Prediction.CurrentHost == null )
			Events.Run( new WeaponFireEvent( new Vector3( -3, 0.2f, 0.2f ) * 35, new Vector3( -1, 0.2f, 0.2f ) * 35 ) );

		Effects?.SetAnimParameter( "bFire", true );
		Sounds.Play( WeaponSound.Shoot, Effects.Position );
	}

	[ConCmd.Server]
	private static void CmdReceivedShootRequest( int index, Vector3 pos, Vector3 forward )
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
			m_Viewmodel = OnRequestViewmodel();

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
