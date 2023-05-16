using Sandbox;

namespace Woosh.Espionage;

public struct DrawTime
{
	public DrawTime( float deploy, float holster )
	{
		Deploy = deploy;
		Holster = holster;
	}

	public float Deploy { get; set; }
	public float Holster { get; set; }
}

public interface ISlotted : IEntity
{
	int Slot { get; }
}

public interface ICarriable : IEntity
{
	DrawTime Draw { get; }

	bool Deployable => true;
	bool Holsterable => true;

	void Deploying();
	void OnDeployed() { }

	void Holstering( bool drop );
	void OnHolstered() { }
}
