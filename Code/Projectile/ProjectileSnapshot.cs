using Sandbox;

namespace Woosh.Espionage;

public struct ProjectileSnapshot
{
	public TimeSince Since;

	public int Attacker;
	public int Weapon;

	public float Force;
	public float Mass;

	public Vector3 Start;
	public Vector3 Forward;
}
