using Editor;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "sandbox_smg" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class SandboxSmgFirearm : Firearm, ISlotted<CarrySlot>, IHave<EntityInfo>
{
	public EntityInfo Item { get; } = new EntityInfo
	{
		Nickname = "SMG",
		Display = "MP5 SMG",
		Brief = "A sub machine gun.",
		Icon = "gavel"
	};

	public SandboxSmgFirearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<CreatedViewModel>(
			static evt => evt.Data.ViewModel.Build()
				.WithModel( Cloud.Model( "facepunch.v_mp5" ) )
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

		Model = Cloud.Model( "facepunch.w_mp5" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Components.RemoveAny<CarriableAimComponent>();
	}

	public CarrySlot Slot => CarrySlot.Front;

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = true,
		RateOfFire = 600,
		Force = 250,

		// Drop on next frame
		Draw = new DrawTime( 1, Game.TickInterval * 2 )
	};

}
