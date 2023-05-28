using System;
using System.Linq;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

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

[Category( "Weapon" ), Icon( "gavel" )]
public abstract partial class Firearm : AnimatedEntity, ICarriable, IPickup, IMutate<CameraSetup>
{
	public IDispatcher Events { get; } = new Dispatcher();
	public EntityStateMachine<Firearm> Machine { get; }

	public Firearm()
	{
		Machine = new EntityStateMachine<Firearm>( this );

		if ( Game.IsClient )
			Events.Register<CreatedViewModel>( static evt => evt.Data.ViewModel.Components.Create<WeaponClientEffectsHandler>() );

		if ( Game.IsServer )
			Events.Register<FirearmRebuildRequest>( Rebuild );
	}

	public IEntityEffects Effects => Components.Get<IEntityEffects>();

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "pickup" );

		PhysicsEnabled = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Components.Create<CarriableAimComponent>();
		Components.Create<WeaponClientEffectsHandler>();

		Components.Create<FirearmCheckAmmoSimulatedEntityState>();
		Components.Create<FirearmShootSimulatedEntityState>();
		Components.Create<FirearmReloadSimulatedEntityState>();

		Rebuild();
	}

	protected override void OnDestroy()
	{
		Events.Dispose();
	}

	public override void Simulate( IClient cl )
	{
		Components.Get<CarriableAimComponent>().Simulate( cl );
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
			mutate.OnPostSetup( ref setup );
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

	public bool Holsterable => Machine.Active == null;

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

	public void OnPostSetup( ref CameraSetup setup )
	{
		foreach ( var component in Components.All().OfType<IMutate<CameraSetup>>() )
		{
			component.OnPostSetup( ref setup );
		}
	}
}
