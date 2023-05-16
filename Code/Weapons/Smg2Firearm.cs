using Editor;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Library( "esp_smg2_firearm" ), Title("SMG2"), Icon("gavel"), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Smg2Firearm : Firearm, ISlotted
{
	private const string VIEW_MODEL = "weapons/smg2/v_espionage_smg2.vmdl";
	private const string WORLD_MODEL = "weapons/smg2/espionage_smg2.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public int Slot => Operator.CarrySlot.Front.Index();
	public override DrawTime Draw => new DrawTime( 1.5f, 1.3f );

	protected override AnimatedEntity OnRequestViewmodel()
	{
		var view = new CompositedViewModel( Events ) { Owner = Owner, Model = Model.Load( VIEW_MODEL ) };
		view.ImportFrom<EspEffectStack>();
		view.SetBodyGroup( "muzzle", 1 );
		return view;
	}
}
