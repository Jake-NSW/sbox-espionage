using Editor;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Library( "esp_dynamic_usable" ), HammerEntity, Category( "World Building" ), Model]
public sealed partial class DynamicUsableEntity : AnimatedEntity, IHave<EntityInfo>, IUse
{
	EntityInfo IHave<EntityInfo>.Item => new EntityInfo()
	{
		Display = Title,
		Description = Description,
		Icon = Icon,
	};

	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
	}

	[Property, Net, Category( "Info" )] public string Title { get; set; }
	[Property, Net, Category( "Info" )] public string Description { get; set; }
	[Property, Net, Category( "Info" )] public string Icon { get; set; }

	public Output OnUse { get; set; }

	// IUse

	bool IUse.IsUsable( Entity user ) => true;

	bool IUse.OnUse( Entity user )
	{
		OnUse.Fire( user );
		return false;
	}
}
