using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "sandbox_pistol" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class SandboxPistolFirearm : Firearm, ISlotted<CarrySlot>, IHave<EntityInfo>
{
	public EntityInfo Item { get; } = new EntityInfo
	{
		Nickname = "Pistol",
		Display = "USP Pistol",
		Brief = "A pistol.",
		Icon = "gavel"
	};

	public SandboxPistolFirearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt => evt.Data.ViewModel.Build()
				.WithModel( Cloud.Model( "facepunch.v_usp" ) )
				.WithComponent<SandboxFirearmViewModelAnimator>()
				.WithAspect<ViewModelEffectsAspect>()
				.WithBodyGroup( "barrel", 2 )
				.WithBodyGroup( "sights", 2 )
				.WithChild( new AnimatedEntity( "models/first_person/first_person_arms.vmdl" ) { EnableViewmodelRendering = true }, true )
		);

		Events.Register<PlayClientEffects<WeaponClientEffects>>(
			evt =>
			{
				if ( evt.Data.Effects == WeaponClientEffects.Attack )
					PlaySound( "rust_pistol.shoot" );
			}
		);
	}

	private const string WORLD_MODEL = "weapons/rust_pistol/rust_pistol.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		Model = Cloud.Model( "facepunch.w_usp" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Components.RemoveAny<CarriableAimComponent>();
	}

	public CarrySlot Slot => CarrySlot.Holster;

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = false,
		RateOfFire = 600,
		Force = 250,

		// Drop on next frame
		Draw = new DrawTime( 1, Game.TickInterval * 2 )
	};

}
