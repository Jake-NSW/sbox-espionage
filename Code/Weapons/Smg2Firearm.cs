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

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		Components.Add( new ViewportEffectsComponent( VIEW_MODEL ) );
	}

	public int Slot => Operator.CarrySlot.Front.Index();
	public override DrawTime Draw => new DrawTime( 1.5f, 1.3f );
}
