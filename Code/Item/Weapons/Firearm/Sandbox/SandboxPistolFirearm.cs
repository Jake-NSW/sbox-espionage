using Editor;
using Sandbox;
using Woosh.Espionage;
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
			static evt => evt.Signal.ViewModel.Build()
				.WithModel( Cloud.Model( "facepunch.v_usp" ) )
				.WithComponent<SandboxFirearmViewModelAnimator>()
				.WithAspect<ViewModelEffectsAspect>()
				.MutateComponent<ViewModelOffsetEffect>( e => e.Aim = new Vector3( -3f, 4.8f, 1f ) )
				.WithBodyGroup( "barrel", 1 )
				.WithBodyGroup( "sights", 1 )
				.WithChild( new AnimatedEntity( "models/first_person/first_person_arms.vmdl" ) { EnableViewmodelRendering = true }, true )
		);

		Events.Register<PlayClientEffects<FirearmClientEffects>>(
			evt =>
			{
				if ( evt.Signal.Effects == FirearmClientEffects.Attack )
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
		// Components.RemoveAny<CarriableAimComponent>();
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
