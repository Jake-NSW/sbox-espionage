using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "esp_mk23_firearm" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Mk23Firearm : Firearm, ISlotted, IHave<EntityInfo>
{
	private const string VIEW_MODEL = "weapons/mk23/v_espionage_mk23.vmdl";
	private const string WORLD_MODEL = "weapons/mk23/espionage_mk23.vmdl";

	public EntityInfo Item { get; } = new EntityInfo
	{
		Display = "MK23",
		Brief = "Heckler & Koch",
		Icon = "gavel",
		Description = "A semi-automatic large framed pistol, chambered in .45 ACP.",
		Group = "weapon"
	};

	public Mk23Firearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt =>
			{
				var model = evt.Data.ViewModel;
				model.FromAspect( new ViewModelEffectsAspect( VIEW_MODEL ) { HipTuck = TuckType.Rotate, AimTuck = TuckType.Push } );
				model.Components.Create<GenericFirearmViewModelAnimator>();
				model.SetMaterialGroup( "tan" );
				model.SetBodyGroup( "muzzle", 0 );
				model.SetBodyGroup( "module", 1 );
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
		IsAutomatic = false,
		IsSilenced = false,
		RateOfFire = 550,
		Force = 400,
		Draw = new DrawTime( 1, 0.6f )
	};

	public int Slot => CarrySlot.Holster.Index();

	private static SoundBank<WeaponClientEffects> Sounds { get; } = new SoundBank<WeaponClientEffects>()
	{
		[WeaponClientEffects.Attack] = "mk23_firing_sound", [WeaponClientEffects.Attack | WeaponClientEffects.Silenced] = "mk23_firing_suppressed_sound",
	};
}
