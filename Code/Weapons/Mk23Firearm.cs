using Editor;
using Sandbox;

namespace Woosh.Espionage;

[Library( "esp_mk23_firearm" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Mk23Firearm : Firearm, IHave<DisplayInfo>, ISlotted
{
	DisplayInfo IHave<DisplayInfo>.Item => new DisplayInfo() { Name = "Mark23", Icon = "gavel" };

	private const string VIEW_MODEL = "weapons/mk23/v_espionage_mk23.vmdl";
	private const string WORLD_MODEL = "weapons/mk23/espionage_mk23.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public int Slot => Operator.CarrySlot.Holster.Index();
	public override DrawTime Draw => new DrawTime( 1, 0.6f );

	protected override AnimatedEntity OnRequestViewmodel()
	{
		var view = new CompositedViewModel( Events ) { Owner = Owner, Model = Model.Load( VIEW_MODEL ) };
		view.ImportFrom<EspEffectStack>();

		// view.SetBodyGroup( "module", 1 );
		view.SetBodyGroup( "muzzle", 1 );

		return view;
	}
}
