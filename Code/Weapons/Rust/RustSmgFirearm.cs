using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "weapon_smg" ), Title( "SMG" ), HammerEntity, EditorModel( WORLD_MODEL )]
public class RustSmgFirearm : Firearm, ISlotted
{
	public RustSmgFirearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt =>
			{
				var view = evt.Data.ViewModel;
				view.Model = Model.Load( VIEW_MODEL );
				view.Components.Create<RustFirearmViewmodelAnimator>();
				view.Components.Create<SandboxViewModelEffect>();
				view.Components.Add( new ViewModelOffsetEffect( new Vector3( -10, 6, 1 ), new Vector3( -10, 6, 1 ) ) );
			}
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
