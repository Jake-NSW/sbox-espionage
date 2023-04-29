namespace Woosh.Espionage;

public readonly struct WeaponFireEvent : IEvent
{
	public Vector3 Recoil { get; }
	public Vector3 Kickback { get; }

	public WeaponFireEvent( Vector3 recoil, Vector3 kickback )
	{
		Recoil = recoil;
		Kickback = kickback;
	}
}
