using Editor;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Library( "esp_mk23_firearm" ), Title( "Mark 23" ), Icon( "gavel" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Mk23Firearm : Firearm, ISlotted
{
	private const string VIEW_MODEL = "weapons/mk23/v_espionage_mk23.vmdl";
	private const string WORLD_MODEL = "weapons/mk23/espionage_mk23.vmdl";

	public Mk23Firearm()
	{
		Events.Register<CreateViewModel>(
			static evt =>
			{
				var view = evt.Data.ViewModel;
				view.Model = Model.Load( VIEW_MODEL );
				view.ImportFrom<CompositedViewModel, EspEffectStack>();
				view.Components.Get<ViewModelTuckEffect>().Variant = TuckType.Rotate;
			}
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

	protected override FirearmSetup OnSetupDefault()
	{
		return new FirearmSetup()
		{
			IsAutomatic = false,
			RateOfFire = 650,
			Draw = new DrawTime( 1, 0.6f )
		};
	}

	protected override SoundBank<WeaponClientEffects> Sounds { get; } = new SoundBank<WeaponClientEffects>()
	{
		[WeaponClientEffects.Attack] = "mk23_firing_sound",
		[WeaponClientEffects.Attack | WeaponClientEffects.Silenced] = "mk23_firing_suppressed_sound",
	};

	public int Slot => CarrySlot.Holster.Index();
}
