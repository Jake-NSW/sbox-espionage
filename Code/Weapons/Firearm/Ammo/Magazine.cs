using Sandbox;

namespace Woosh.Espionage;

public abstract class Magazine : ModelEntity, IFirearmAmmoProvider
{
	protected Magazine( int capacity ) { Capacity = capacity; }

	public int Spare { get; }
	public int Capacity { get; }

	public bool Add( int? type = null )
	{
		if ( Spare == Capacity )
			return false;

		return true;
	}

	public (bool Valid, int Type) Consume( int amount )
	{
		return default;
	}
}
