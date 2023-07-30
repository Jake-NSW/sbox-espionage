using Sandbox;

namespace Woosh.Espionage;

public abstract class ShotgunShell : ModelEntity, IFirearmAmmoProvider
{
	public int Spare => 1;

	public int Capacity => 1;

	public bool Add( int? type = null )
	{
		return false;
	}

	public (bool Valid, int Type) Consume( int amount )
	{
		throw new System.NotImplementedException();
	}
}
