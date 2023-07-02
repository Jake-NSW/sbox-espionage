using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "weapon_pistol" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class RustPistolFirearm : Firearm, ISlotted, IHave<EntityInfo>
{
	public EntityInfo Item { get; } = new EntityInfo
	{
		Display = "Makeshift Pistol",
		Brief = "Made with shit",
		Icon = "gavel"
	};

	public RustPistolFirearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt => evt.Data.ViewModel.Build()
				.WithModel( Model.Load( VIEW_MODEL ) )
				.WithComponent<RustFirearmViewmodelAnimator>()
				.WithComponent<SandboxViewModelEffect>()
				.WithComponent( new ViewModelOffsetEffect( new Vector3( -10, 6, 1 ), new Vector3( -10, 6, 1 ) ) )
				.WithComponent<ViewModelPitchOffsetEffect>()
		);

		Events.Register<PlayClientEffects<WeaponClientEffects>>(
			evt =>
			{
				if ( evt.Data.Effects == WeaponClientEffects.Attack )
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
	}

	public int Slot => CarrySlot.Holster.Index();

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = false,
		RateOfFire = 600,
		Force = 250,

		// Drop on next frame
		Draw = new DrawTime( 1, Game.TickInterval * 2 )
	};
}
