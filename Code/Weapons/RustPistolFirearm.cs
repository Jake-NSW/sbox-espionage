using Editor;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Library( "dm_pistol" ), Title( "Pistol" ), Icon( "gavel" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class RustPistolFirearm : Firearm, ISlotted
{
	public RustPistolFirearm()
	{
		Events.Register<CreateViewModel>(
			evt =>
			{
				var view = evt.Data.ViewModel;
				view.Model = Model.Load( VIEW_MODEL );
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

		Components.Create<CarriableEffectsComponent>();
		Components.Create<RustFirearmViewmodelAnimator>();
	}

	public int Slot => CarrySlot.Holster.Index();

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = false,
		RateOfFire = 600,
		Draw = new DrawTime( 1, 0.1f )
	};
}
