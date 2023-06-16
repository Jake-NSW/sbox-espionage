using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library, Title( "Hands" ), Icon( "pan_tool" )]
public sealed partial class PlayerHands : MeleeWeapon
{
	public PlayerHands()
	{
		Events.Register<CreatedViewModel>(
			evt =>
			{
				var view = evt.Data.ViewModel;
				view.Model = Model.Load( "weapons/hands/v_espionage_hands.vmdl" );
				view.Components.Create<GenericFirearmViewModelAnimator>();
			}
		);
	}

	public override void Spawn()
	{
		Transmit = TransmitType.Owner;
	}
}
