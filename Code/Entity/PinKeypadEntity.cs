using Editor;
using Sandbox;
using Woosh.Data;

namespace Woosh.Espionage;

[Library( "esp_keypad" ), HammerEntity, Category( "Gameplay" ), Icon( "pin" ), Model]
public sealed partial class PinKeypadEntity : AnimatedEntity, IUse, IProfiled
{
	public Profile? Profile { get; } = new Profile( "Keypad" ) { Icon = "pin", Brief = "Use", Binding = "E" };

	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
	}

	public bool OnUse( Entity user )
	{
		Log.Info("Using");
		return false;
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}
}
