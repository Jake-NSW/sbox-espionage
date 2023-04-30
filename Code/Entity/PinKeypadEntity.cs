using Editor;
using Sandbox;

namespace Woosh.Espionage;

[Library( "esp_keypad" ), HammerEntity, Title( "Keypad" ), Category( "Gameplay" ), Icon( "pin" ), Model]
public sealed partial class PinKeypadEntity : AnimatedEntity, IUse
{
	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
	}

	public bool OnUse( Entity user )
	{
		return false;
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}
}
