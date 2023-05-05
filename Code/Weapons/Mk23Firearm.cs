using Editor;
using Sandbox;

namespace Woosh.Espionage;

[Library( "esp_mk23_firearm" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Mk23Firearm : Weapon, IHave<DisplayInfo>, ISlotted
{
	DisplayInfo IHave<DisplayInfo>.Item => new DisplayInfo() { Name = "Mark23" };

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
		return view;

		view.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
		view.Add( new ViewModelSwayEffect( 1, 1.3f ) );
		view.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
		view.Add( new ViewModelStrafeOffsetEffect() { Damping = 6, RollMultiplier = 1, AxisMultiplier = 10 } );
		view.Add( new ViewModelDeadzoneSwayEffect( new Vector2( 10, 10 ) ) { AimingOnly = true, AutoCenter = false, Damping = 8 } );
		view.Add( new ViewModelPitchOffsetEffect( 5, 4 ) { Damping = 15 } );
		view.Add( new ViewModelRecoilEffect() );
		view.Add( new ViewModelTuckEffect() );

		view.SetBodyGroup( "module", 1 );
		// view.SetBodyGroup( "muzzle", 1 );

		return view;
	}
}
