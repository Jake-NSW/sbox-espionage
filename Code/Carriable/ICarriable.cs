using Woosh.Signals;

namespace Woosh.Espionage;

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
