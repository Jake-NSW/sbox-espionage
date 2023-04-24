using Sandbox;

namespace Woosh.Espionage;

public class Item : Entity, IPickup
{
	public Item()
	{
		Transmit = TransmitType.Pvs;
	}

	void IPickup.OnPickup( Entity carrier )
	{
		Game.AssertServer();
		
		Owner = carrier;
		EnableDrawing = false;

		SetParent( carrier, "weapon_attach", Transform.Zero );
	}

	void IPickup.OnDrop()
	{
		Game.AssertServer();

		SetParent(null);
		
		Position = Owner.AimRay.Position + Vector3.Down * 16;
		Velocity += Owner.AimRay.Forward * 25;

		Owner = null;
	}
}

public interface IPickup
{
	void OnPickup( Entity carrier );
	void OnDrop();
}
