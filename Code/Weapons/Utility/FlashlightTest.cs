using Editor;
using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage.Utility;

[Library( "esp_flash_light_test" ), Category( "Utility" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class FlashlightTest : ObservableAnimatedEntity, ICarriable, IPickup, ISlotted<CarrySlot>, IHave<EntityInfo>
{
	private const string WORLD_MODEL = "weapons/mk23/espionage_mk23.vmdl";

	public FlashlightTest()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt => evt.Signal.ViewModel.Build()
				.WithModel( Model.Load( "weapons/rust_flashlight/v_rust_flashlight.vmdl" ) )
				.WithAspect( new ViewModelEffectsAspect() )
				.WithComponent( new RustFirearmViewModelAnimator() )
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

	public CarrySlot Slot => CarrySlot.Utility;

	public EntityInfo Item => new EntityInfo()
	{
		Nickname = "Light",
		Display = "Flashlight Test",
		Brief = "about it...",
		Icon = "light",
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
