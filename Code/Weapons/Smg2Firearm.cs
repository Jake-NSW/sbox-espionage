using Editor;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Library( "esp_smg2_firearm" ), Title( "SMG2" ), Icon( "gavel" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Smg2Firearm : Firearm, ISlotted
{
	private const string VIEW_MODEL = "weapons/smg2/v_espionage_smg2.vmdl";
	private const string WORLD_MODEL = "weapons/smg2/espionage_smg2.vmdl";

	public Smg2Firearm()
	{
		Events.Register<CreateViewModel>(
			static evt => evt.Data.ViewModel.FromAspect( new ViewModelEffectsAspect( VIEW_MODEL ) { AimTuck = TuckType.Rotate, HipTuck = TuckType.Hug } )
		);
	}

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		Components.Create<CarriableEffectsComponent>();
		Components.Create<GenericFirearmViewmodelAnimator>();
	}

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = true,
		RateOfFire = 750,
		Draw = new DrawTime( 1.5f, 1.3f )
	};

	protected override SoundBank<WeaponClientEffects> Sounds { get; } = new SoundBank<WeaponClientEffects>()
	{
		[WeaponClientEffects.Attack] = "mk23_firing_sound", [WeaponClientEffects.Attack | WeaponClientEffects.Silenced] = "smg2_firing_suppressed_sound",
	};

	public int Slot => CarrySlot.Front.Index();
}
