using Sandbox;

namespace Woosh.Espionage;

public ref struct ViewModelSetup
{
	internal ViewModelSetup(  Entity owner )
	{
		m_Owner = owner;
		Aim = 0;
		Offset = Vector3.Zero;
		Angles = Rotation.Identity;
	}

	// Read Only

	public float Aim;

	private readonly Entity m_Owner;
	public Vector3 Velocity => m_Owner.Velocity;

	// Mutable

	public Vector3 Offset;
	public Rotation Angles;
}
