using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Library, Title("Hands"), Icon("pan_tool")]
public sealed partial class PlayerHands : AnimatedEntity, ICarriable, IObservableEntity
{
	public Dispatcher Events { get; } = new Dispatcher();
	
	public override void Spawn()
	{
		Transmit = TransmitType.Owner;
		Components.Add( new CarriableEffectsComponent( Model.Load(  "weapons/hands/v_espionage_hands.vmdl"  ) ) );
	}

	// ICarriable

	public DrawTime Draw => new DrawTime( 1, 0.5f );

	void ICarriable.Deploying()
	{
		Events.Run( new DeployingEntity( this ), this );
		EnableDrawing = true;
	}

	void ICarriable.Holstering( bool drop )
	{
		Events.Run( new HolsteringEntity( this, drop ), this );
	}

	void ICarriable.OnHolstered()
	{
		Events.Run( new HolsteredEntity( this ), this );
		EnableDrawing = false;
	}

}
