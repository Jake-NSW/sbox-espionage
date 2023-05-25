using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public struct DrawTime
{
	public DrawTime( float deploy, float holster )
	{
		Deploy = deploy;
		Holster = holster;
	}

	public float Deploy;
	public float Holster;
}

public interface ISlotted : IEntity
{
	int Slot { get; }
}

public interface ICarriable : IObservableEntity
{
	DrawTime Draw { get; }

	bool Deployable => true;
	bool Holsterable => true;

	void Deploying();
	void OnDeployed() { }

	void Holstering( bool drop );
	void OnHolstered() { }
}
