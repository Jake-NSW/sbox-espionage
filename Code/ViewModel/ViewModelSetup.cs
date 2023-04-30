using Sandbox;

namespace Woosh.Espionage;

public ref struct ViewModelSetup
{
	internal ViewModelSetup( AnimatedEntity entity, Entity owner, Transform initial, float aim )
	{
		Entity = entity;
		Owner = owner;
		Initial = initial;
		Aim = aim;
		Position = Initial.Position;
		Rotation = Initial.Rotation;
	}

	// Read Only

	public float Aim { get; }
	
	public AnimatedEntity Entity { get; }
	public Entity Owner { get; }
	
	public Transform Initial { get; }
	public Vector3 Velocity => Owner.Velocity;

	// Mutable

	public Vector3 Position;
	public Rotation Rotation;
}
