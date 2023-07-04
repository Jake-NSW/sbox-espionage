using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage.Utility;

[Library( "esp_long_stick_gun" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class LongStickGun : ObservableAnimatedEntity, ICarriable, IPickup, ISlotted<CarrySlot>, IHave<EntityInfo>
{
	private const string WORLD_MODEL = "weapons/mk23/espionage_mk23.vmdl";

	public LongStickGun()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt => evt.Data.ViewModel.Build()
				.WithModel( Model.Load( "weapons/debug/v_long_gun.vmdl" ) )
				.WithAspect( new ViewModelEffectsAspect() )
				.WithComponent( new GenericFirearmViewModelAnimator() )
				.WithMaterialGroup( "chrome" )
				.WithBodyGroup( "muzzle", 0 )
				.WithBodyGroup( "module", 1 )
		);
	}

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "pickup" );

		PhysicsEnabled = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Model = Cloud.Model( "woosh.mdl_esp_mk23" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public CarrySlot Slot => CarrySlot.Grenade;

	public EntityInfo Item => new EntityInfo()
	{
		Nickname = "Tuck",
		Display = "Tuck Debug",
		Brief = "A gun with a long stick",
		Icon = "gavel",
	};

	// IPickup

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

	// ICarriable

	public DrawTime Draw => new DrawTime( 1, 0.6f );

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
