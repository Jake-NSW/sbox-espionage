using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public abstract class MeleeWeapon : ObservableAnimatedEntity, ICarriable
{
	// ICarriable
	
	public virtual DrawTime Draw => new DrawTime( 1, 0.5f );

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
