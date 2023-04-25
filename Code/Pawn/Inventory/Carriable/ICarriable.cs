using Sandbox;

namespace Woosh.Espionage;

public interface ICarriable : IEntity
{
	bool Deployable => true;
	bool Holsterable => true;

	void Deploying();
	void OnDeployed() { }
	
	void Holstering( bool drop );
	void OnHolstered() { }
}
