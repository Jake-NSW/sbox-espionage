using Editor;
using Sandbox;

namespace Woosh.Espionage;

[Library( "esp_door" ), HammerEntity, Category( "Gameplay" ), Icon( "door_front" ), Model]
public sealed partial class PushableDoorEntity : AnimatedEntity, IPushable
{
	public PushableDoorEntity()
	{
		Transmit = TransmitType.Pvs;
	}

	public override void Spawn()
	{
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		EnableAllCollisions = true;
		UsePhysicsCollision = true;
	}

	public void Push( float amount ) { }
}
