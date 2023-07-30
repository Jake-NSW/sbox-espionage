using Editor;
using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "esp_smg2_firearm" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Smg2Firearm : Firearm, ISlotted<CarrySlot>, IHave<EntityInfo>
{
	private const string WORLD_MODEL = "weapons/smg2/espionage_smg2.vmdl";

	public EntityInfo Item { get; } = new EntityInfo()
	{
		Nickname = "SMG II",
		Display = "Prototype SMG II",
		Brief = "Heckler & Koch",
		Icon = "gavel",
		Description = "A prototype supersonic sub machine gun"
	};

	public Smg2Firearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt => evt.Signal.ViewModel.Build()
				.WithModel( Cloud.Model( "woosh.mdl_esp_vsmg2" ) )
				.WithAspect( new ViewModelEffectsAspect() )
				.WithComponent( new EspionageFirearmViewModelAnimator() )
				.WithBodyGroup( "muzzle", 1 )
		);

		Events.Register<PlayClientEffects<FirearmEffects>>(
			static evt => Sounds.Play( evt.Signal.Effects, Game.LocalPawn.AimRay.Position )
		);
	}

	public override void Spawn()
	{
		base.Spawn();
		Model = Cloud.Model( "woosh.mdl_esp_smg2" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = true,
		IsSilenced = true,
		RateOfFire = 700,
		Force = 250,
		Spread = 0.2f,
		Draw = new DrawTime( 1.5f, 1.3f )
	};

	public CarrySlot Slot => CarrySlot.Front;

	private static SoundBank<FirearmEffects> Sounds { get; } = new SoundBank<FirearmEffects>()
	{
		[FirearmEffects.Attack] = "mk23_firing_sound", [FirearmEffects.Attack | FirearmEffects.Silenced] = "smg2_firing_suppressed_sound",
	};
}
