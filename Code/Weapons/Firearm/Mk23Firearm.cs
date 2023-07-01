using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "esp_mk23_firearm" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Mk23Firearm : Firearm, ISlotted, IHave<EntityInfo>
{
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
			static evt => evt.Data.ViewModel.Build()
				.WithModel( Cloud.Model( "woosh.mdl_esp_vmk23" ) )
				.WithAspect( new ViewModelEffectsAspect() )
				.WithComponent( new GenericFirearmViewModelAnimator() )
				.WithMaterialGroup( "tan" )
				.WithBodyGroup( "muzzle", 0 )
				.WithBodyGroup( "module", 1 )
		);

		Events.Register<PlayClientEffects<WeaponClientEffects>>(
			static evt => Sounds.Play( evt.Data.Effects, Game.LocalPawn.AimRay.Position )
		);
	}

	public override void Spawn()
	{
		base.Spawn();

		Model = Cloud.Model( "woosh.mdl_esp_mk23" );
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
