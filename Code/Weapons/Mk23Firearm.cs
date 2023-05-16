using Editor;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Library( "esp_mk23_firearm" ), Title( "Mark 23" ), Icon( "gavel" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Mk23Firearm : Firearm, ISlotted
{
	private const string VIEW_MODEL = "weapons/mk23/v_espionage_mk23.vmdl";
	private const string WORLD_MODEL = "weapons/mk23/espionage_mk23.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		Components.Add( new CarriableEffectsComponent( Model.Load( VIEW_MODEL ) ) );
		Components.Create<GenericFirearmViewmodelAnimator>();
	}

	public int Slot => Operator.CarrySlot.Holster.Index();
	public override DrawTime Draw => new DrawTime( 1, 0.6f );
}
