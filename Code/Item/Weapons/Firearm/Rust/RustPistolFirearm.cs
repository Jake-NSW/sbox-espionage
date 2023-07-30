using Editor;
using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "weapon_pistol" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class RustPistolFirearm : Firearm, ISlotted<CarrySlot>, IHave<EntityInfo>
{
	public EntityInfo Item { get; } = new EntityInfo
	{
		Nickname = "SAP",
		Display = "Makeshift Pistol",
		Brief = "Made with shit",
		Icon = "gavel"
	};

	public RustPistolFirearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt => evt.Signal.ViewModel.Build()
				.WithModel( Model.Load( VIEW_MODEL ) )
				.WithComponent<RustFirearmViewModelAnimator>()
				.WithComponent<SandboxViewModelEffect>()
				.WithComponent( new ViewModelOffsetEffect( new Vector3( -10, 6, 1 ), new Vector3( -10, 6, 1 ) ) )
				.WithComponent<ViewModelPitchOffsetEffect>()
		);

		Events.Register<PlayClientEffects<FirearmEffects>>(
			evt =>
			{
				if ( evt.Signal.Effects == FirearmEffects.Attack )
					PlaySound( "rust_pistol.shoot" );
			}
		);
	}

	private const string VIEW_MODEL = "weapons/rust_pistol/v_rust_pistol.vmdl";
	private const string WORLD_MODEL = "weapons/rust_pistol/rust_pistol.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Components.RemoveAny<CarriableAimComponent>();
	}

	public CarrySlot Slot => CarrySlot.Holster;

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = false,
		RateOfFire = 600,
		Force = 250,

		// Drop on next frame
		Draw = new DrawTime( 1, Game.TickInterval * 2 )
	};
}
