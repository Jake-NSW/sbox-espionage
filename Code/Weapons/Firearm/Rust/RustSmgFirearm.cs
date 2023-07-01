using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "weapon_smg" ), Title( "SMG" ), Description( "Pew Pew" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class RustSmgFirearm : Firearm, ISlotted, IHave<EntityInfo>
{
	public EntityInfo Item { get; } = new EntityInfo()
	{
		Display = "SMG",
		Brief = "Pew Pew",
		Icon = "gavel"
	};

	public RustSmgFirearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt => evt.Data.ViewModel.Build()
				.WithModel( Model.Load( VIEW_MODEL ) )
				.WithComponent( new RustFirearmViewmodelAnimator() )
				.WithComponent( new SandboxViewModelEffect() )
				.WithComponent( new ViewModelOffsetEffect( new Vector3( -10, 6, 1 ), new Vector3( -10, 6, 1 ) ) )
		);

		Events.Register<PlayClientEffects<WeaponClientEffects>>(
			evt =>
			{
				if ( evt.Data.Effects == WeaponClientEffects.Attack )
					PlaySound( "rust_smg.shoot" );
			}
		);
	}

	private const string VIEW_MODEL = "weapons/rust_smg/v_rust_smg.vmdl";
	private const string WORLD_MODEL = "weapons/rust_smg/rust_smg.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	protected override FirearmSetup Default { get; } = new FirearmSetup()
	{
		IsAutomatic = true,
		RateOfFire = 650,
		Force = 250,
		Draw = new DrawTime( 1, Game.TickInterval * 2 )
	};

	public int Slot => CarrySlot.Front.Index();
}
