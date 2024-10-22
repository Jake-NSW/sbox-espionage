﻿using System;
using System.Linq;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

[Flags]
public enum FirearmEffects
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

	public float Weight;
	public float Force;
	public float Spread;
	public float Control;
	public float Mobility;
	public float Range;
	public float Accuracy;

	public DrawTime Draw;
}

[Category( "Weapon" ), Icon( "gavel" )]
public abstract partial class Firearm : ObservableAnimatedEntity, ICarriable, IPickup, IMutate<InputContext>, IMutate<CameraSetup>
{
	public EntityStateMachine<Firearm> Machine { get; }

	protected Firearm()
	{
		Machine = new EntityStateMachine<Firearm>( this );

		if ( Game.IsClient )
			Events.Register<CreatedViewModel>( static evt => evt.Signal.ViewModel.Components.Create<FirearmEffectsHandler>() );

		if ( Game.IsServer )
			Events.Register<FirearmRebuildRequest>( Rebuild );
	}

	public AnimatedEntity Effects => Components.Get<IEntityEffects>()?.Target ?? this;

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "pickup" );

		PhysicsEnabled = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Components.Create<CarriableAimComponent>();
		Components.Create<FirearmEffectsHandler>();

		Components.Create<CheckAmmoFirearmState>();
		Components.Create<ShootFirearmState>();
		Components.Create<ReloadFirearmState>();

		Rebuild();
	}

	protected override void OnDestroy()
	{
		Events.Dispose();
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
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
		var setup = Default;

		foreach ( var mutate in Components.All().OfType<IMutate<FirearmSetup>>() )
		{
			mutate.OnMutate( ref setup );
		}

		n_Setup = setup;
		Events.Run( new FirearmSetupApplied( setup ) );
	}

	private void OnSetupChanged( FirearmSetup old, FirearmSetup value )
	{
		Events.Run( new FirearmSetupApplied( value ) );
	}

	protected abstract FirearmSetup Default { get; }

	// Pickup

	public bool Holsterable => !Machine.InState;

	void IPickup.OnPickup( Entity carrier )
	{
		EnableAllCollisions = false;
		PhysicsEnabled = false;
	}

	void IPickup.OnDrop()
	{
		EnableAllCollisions = true;
		PhysicsEnabled = true;

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

	public void OnMutate( ref InputContext setup )
	{
		// Post-Mutate Input Context
		foreach ( var component in Components.All().OfType<IMutate<InputContext>>() )
		{
			component.OnMutate( ref setup );
		}
	}

	public void OnMutate( ref CameraSetup setup )
	{
		// Post-Mutate Camera Setup
		foreach ( var component in Components.All().OfType<IMutate<CameraSetup>>() )
		{
			component.OnMutate( ref setup );
		}
	}
}
