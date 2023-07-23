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
