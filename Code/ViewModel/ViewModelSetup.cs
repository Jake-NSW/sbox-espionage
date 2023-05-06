using Sandbox;

namespace Woosh.Espionage;

public ref struct ViewModelSetup
{
	internal ViewModelSetup( Entity owner )
	{
		m_Owner = owner;
		Aim = 0;
		Offset = Vector3.Zero;
		Angles = Rotation.Identity;
	}

	private readonly Entity m_Owner;
	public Vector3 Velocity => m_Owner.Velocity;

	// Mutable

	public float Aim;

	public Vector3 Offset;
	public Rotation Angles;
}
