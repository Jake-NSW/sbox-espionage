using System;
using System.Linq;
using System.Runtime.CompilerServices;
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

public struct FirearmSetup
{
	public bool IsAutomatic;
	public bool IsSilenced;

	public float RateOfFire; // RPM
	public float Control;
	public float Mobility;
	public float Range;
	public float Accuracy;

	public DrawTime Draw;
}

public abstract partial class Firearm : AnimatedEntity, ICarriable, IPickup, IObservableEntity
{
	public Dispatcher Events { get; } = new Dispatcher();
	public EntityStateMachine<Firearm> Machine { get; }

	public Firearm()
	{
		Machine = new EntityStateMachine<Firearm>( this );
	}

	public IEntityEffects Effects => Components.Get<IEntityEffects>();

	public override void Spawn()
	{
		base.Spawn();

		PhysicsEnabled = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Components.Create<FirearmShootSimulatedEntityState>();
		Components.Create<FirearmReloadSimulatedEntityState>();

		Rebuild();
	}

	public override void Simulate( IClient cl )
	{
		Machine.Simulate( cl );
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

		foreach ( var mutate in Components.All().OfType<IMutate<FirearmSetup>>() )
		{
			mutate.OnPostSetup( ref setup );
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

	DrawTime ICarriable.Draw => Setup.Draw;

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
