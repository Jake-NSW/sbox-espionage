using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library, Title( "Hands" ), Icon( "pan_tool" )]
public sealed partial class PlayerHands : MeleeWeapon, ISlotted<CarrySlot>, IHave<EntityInfo>
{
	public PlayerHands()
	{
		Events.Register<CreatedViewModel>(
			evt =>
			{
				var view = evt.Data.ViewModel;
				view.Model = Model.Load( "weapons/hands/v_espionage_hands.vmdl" );
				view.Components.Create<GenericFirearmViewModelAnimator>();
				view.Build().WithAspect( new ViewModelEffectsAspect() ).WithoutAnyComponent<ViewModelPitchOffsetEffect>();
			}
		);
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( "shoot" ) && Game.IsClient )
		{
			Components.Get<AppliedViewModelEntityEffects>()?.ViewModel?.SetAnimParameter( "bAttackHit", true );
		}
	}

	public override void Spawn()
	{
		Transmit = TransmitType.Owner;
	}

	public CarrySlot Slot => CarrySlot.Utility;
	public EntityInfo Item { get; } = new EntityInfo() { Nickname = "Hands", Display = "Hands" };
}
