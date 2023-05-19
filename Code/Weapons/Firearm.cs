using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Flags]
public enum WeaponClientEffects
{
	Attack,
	Silenced,
	Reload
}

public interface IMutateFirearmSetup
{
	void OnPostFirearmSetup( ref FirearmSetup setup );
}

public struct FirearmSetup
{
	public bool IsAutomatic;
	public bool IsSilenced;

	public float RateOfFire; // RPM
	public float Control;
	public float Mobility;
	public float Range;
	public float Accuracy;
}

public abstract partial class Firearm : AnimatedEntity, ICarriable, IPickup, IObservableEntity
{
	public Dispatcher Events { get; } = new Dispatcher();

	public IEntityEffects Effects => Components.Get<IEntityEffects>();

	public override void Spawn()
	{
		base.Spawn();

		PhysicsEnabled = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Rebuild();
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

	// Setup

	public FirearmSetup Setup => n_Setup;

	[Net, SkipHotload, Change( nameof(OnSetupChanged) )]
	private FirearmSetup n_Setup { get; set; }

	[Event.Hotload]
	private void OnHotload()
	{
		if ( Game.IsServer )
			Rebuild();
	}

	public void Rebuild()
	{
		Game.AssertServer();
		var setup = OnSetupDefault();

		foreach ( var mutate in Components.All().OfType<IMutateFirearmSetup>() )
		{
			mutate.OnPostFirearmSetup( ref setup );
		}

		n_Setup = setup;
		Events.Run( new FirearmSetupApplied( setup ) );
	}

	private void OnSetupChanged( FirearmSetup old, FirearmSetup value )
	{
		Events.Run( new FirearmSetupApplied( value ) );
	}

	protected abstract FirearmSetup OnSetupDefault();

	// Shoot

	protected virtual SoundBank<WeaponClientEffects> Sounds { get; } = new SoundBank<WeaponClientEffects>() { [WeaponClientEffects.Attack] = "player_use_fail" };

	[Net, Predicted, Local] private TimeSince n_SinceLastShot { get; set; }

	public bool IsFireable( bool checkInput = false )
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

		Events.Run( new WeaponFired( new Vector3( -3, 0.2f, 0.2f ) * 35, new Vector3( -1, 0.2f, 0.2f ) * 35 ) );

		// Play Effects
		PlayClientEffects( WeaponClientEffects.Attack );

		// Owner, Shoot from View Model
		if ( IsLocalPawn )
		{
			var muzzle = Effects?.Target?.GetAttachment( "muzzle" ) ?? Owner.Transform;
			CmdReceivedShootRequest( NetworkIdent, muzzle.Position, muzzle.Rotation.Forward );
			return;
		}

		// No Owner, Shoot from World Model
		if ( Owner == null && Game.IsServer ) { }
	}

	[ClientRpc]
	private void PlayClientEffects( WeaponClientEffects effects )
	{
		if ( Prediction.CurrentHost == null )
			Events.Run( new WeaponFired( new Vector3( -3, 0.2f, 0.2f ) * 35, new Vector3( -1, 0.2f, 0.2f ) * 35 ) );

		Sounds.Play( effects, Owner?.AimRay.Position ?? Position );
		Events.Run( new PlayClientEffects<WeaponClientEffects>( effects ) );
	}

	[ConCmd.Server]
	private static void CmdReceivedShootRequest( int indent, Vector3 pos, Vector3 forward )
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

	public virtual DrawTime Draw => new DrawTime( 1, 1 );

	void ICarriable.Deploying()
	{
		if ( Game.IsServer )
			EnableDrawing = true;
	}

	void ICarriable.Holstering( bool drop ) { }

	void ICarriable.OnHolstered()
	{
		if ( Game.IsServer )
			EnableDrawing = false;
	}
}
