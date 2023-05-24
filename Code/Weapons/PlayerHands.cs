using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Library, Title( "Hands" ), Icon( "pan_tool" )]
public sealed partial class PlayerHands : AnimatedEntity, ICarriable, IObservableEntity
{
	public Dispatcher Events { get; } = new Dispatcher();

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

	// ICarriable

	public DrawTime Draw => new DrawTime( 1, 0.5f );

	void ICarriable.Deploying()
	{
		if ( Game.IsServer )
			EnableDrawing = true;
	}

	void ICarriable.Holstering( bool drop ) { }

	void ICarriable.OnHolstered()
	{
		if ( Game.IsServer )
			EnableDrawing = false;
	}

}
