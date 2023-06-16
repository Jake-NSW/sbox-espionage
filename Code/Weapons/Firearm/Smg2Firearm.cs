using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "esp_smg2_firearm" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Smg2Firearm : Firearm, ISlotted, IHave<EntityInfo>
{
	private const string VIEW_MODEL = "weapons/smg2/v_espionage_smg2.vmdl";
	private const string WORLD_MODEL = "weapons/smg2/espionage_smg2.vmdl";

	public EntityInfo Item { get; } = new EntityInfo()
	{
		Display = "SMG II",
		Brief = "Heckler & Koch",
		Icon = "gavel",
		Description = "A prototype supersonic sub machine gun"
	};

	public Smg2Firearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt =>
			{
				var model = evt.Data.ViewModel;
				model.FromAspect( new ViewModelEffectsAspect( VIEW_MODEL ) { AimTuck = TuckType.Rotate, HipTuck = TuckType.Hug } );
				model.Components.Create<GenericFirearmViewModelAnimator>();
				model.SetBodyGroup( "muzzle", 1 );
			}
		);

		Events.Register<PlayClientEffects<WeaponClientEffects>>(
			static evt => Sounds.Play( evt.Data.Effects, Game.LocalPawn.AimRay.Position )
		);
	}

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( WORLD_MODEL );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = true,
		IsSilenced = true,
		RateOfFire = 700,
		Force = 250,
		Spread = 2,
		Draw = new DrawTime( 1.5f, 1.3f )
	};

	public int Slot => CarrySlot.Front.Index();

	private static SoundBank<WeaponClientEffects> Sounds { get; } = new SoundBank<WeaponClientEffects>()
	{
		[WeaponClientEffects.Attack] = "mk23_firing_sound", [WeaponClientEffects.Attack | WeaponClientEffects.Silenced] = "smg2_firing_suppressed_sound",
	};
}
