using Editor;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "weapon_shotgun" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class RustShotgunFirearm : Firearm, ISlotted<CarrySlot>, IHave<EntityInfo>
{
	public EntityInfo Item { get; } = new EntityInfo()
	{
		Nickname = "Pump",
		Display = "Makeshift Shotgun",
		Brief = "Made with love",
		Icon = "gavel"
	};

	public RustShotgunFirearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt => evt.Signal.ViewModel.Build()
				.WithModel( Model.Load( VIEW_MODEL ) )
				.WithComponent( new RustFirearmViewModelAnimator() )
				.WithComponent( new SandboxViewModelEffect() )
				.WithComponent( new ViewModelOffsetEffect( new Vector3( -10, 6, 1 ), new Vector3( -10, 6, 1 ) ) )
				.WithComponent( new ViewModelPitchOffsetEffect() )
		);

		Events.Register<PlayClientEffects<FirearmClientEffects>>(
			evt =>
			{
				if ( evt.Signal.Effects == FirearmClientEffects.Attack )
					PlaySound( "rust_pumpshotgun.shoot" );
			}
		);
	}

	private const string VIEW_MODEL = "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
	private const string WORLD_MODEL = "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl";

	public CarrySlot Slot => CarrySlot.Back;

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		// Use Shotgun Simulated Reload State (Continuous reloading)
		Components.Replace<ReloadFirearmState, ReloadShotgunState>();
		Components.RemoveAny<CarriableAimComponent>();
	}

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = false,
		RateOfFire = 70,
		Force = 250,

		// Drop on next frame
		Draw = new DrawTime( 1, Game.TickInterval * 2 )
	};
}
