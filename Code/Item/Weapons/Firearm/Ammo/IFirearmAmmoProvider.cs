using Sandbox;

namespace Woosh.Espionage;

// "TYPE" Is used in the sense of readonly projectile data. 

public interface IFirearmAmmoProvider : IEntity, IPickup
{
	int Spare { get; }
	int Capacity { get; }

	bool Add( int? type = null );
	(bool Valid, int Type) Consume( int amount );
}
