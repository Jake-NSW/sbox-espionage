using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "weapon_shotgun" ), Title( "Shotgun" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class RustShotgunFirearm : Firearm, ISlotted
{
	public RustShotgunFirearm()
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
				view.Components.Add( new ViewModelOffsetEffect( new Vector3( -10, 6, 1 ), new Vector3( -5, 17, 2.4f ) ) );
			}
		);

		Events.Register<PlayClientEffects<WeaponClientEffects>>(
			evt =>
			{
				if ( evt.Data.Effects == WeaponClientEffects.Attack )
					PlaySound( "rust_pumpshotgun.shoot" );
			}
		);
	}

	private const string VIEW_MODEL = "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
	private const string WORLD_MODEL = "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public int Slot => CarrySlot.Back.Index();

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = false,
		RateOfFire = 90,

		// Drop on next frame
		Draw = new DrawTime( 1, Game.TickInterval * 2 )
	};
}
