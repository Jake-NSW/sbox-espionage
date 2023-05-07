using Editor;
using Sandbox;

namespace Woosh.Espionage;

[Library( "esp_rust_pistol" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class RustPistolFirearm : Firearm, IHave<DisplayInfo>, ISlotted
{
	DisplayInfo IHave<DisplayInfo>.Item => new DisplayInfo() { Name = "Rust Pistol", Icon = "gavel" };

	private const string VIEW_MODEL = "weapons/rust_pistol/v_rust_pistol.vmdl";
	private const string WORLD_MODEL = "weapons/rust_pistol/rust_pistol.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public int Slot => Operator.CarrySlot.Holster.Index();
	public override DrawTime Draw => new DrawTime( 1, 0.2f );

	protected override AnimatedEntity OnRequestViewmodel()
	{
		var view = new CompositedViewModel( Events ) { Owner = Owner, Model = Model.Load( VIEW_MODEL ) };
		view.ImportFrom<EspEffectStack>();
		return view;
	}
}
