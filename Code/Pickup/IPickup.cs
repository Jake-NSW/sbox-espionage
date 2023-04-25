using Sandbox;

namespace Woosh.Espionage;

public sealed class Item : AnimatedEntity, IPickup
{
	public Item()
	{
		Transmit = TransmitType.Pvs;
	}

	void IPickup.OnPickup( Entity carrier ) { }

	void IPickup.OnDrop()
	{
		Position = Owner.AimRay.Position + Vector3.Down * 16;
		Velocity += Owner.AimRay.Forward * 25;
	}
}

public interface IPickup
{
	void OnPickup( Entity carrier );
	void OnDrop();
}
