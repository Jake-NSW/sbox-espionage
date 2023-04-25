using Sandbox;

namespace Woosh.Espionage;

public interface IPickup
{
	void OnPickup( Entity carrier ) { }
	void OnDrop() { }
}
