using Sandbox;

namespace Woosh.Espionage;

[Library( "esp_mk23_12r_mag" )]
public sealed class Mk23StandardMagazine : FirearmMagazine, IHave<EntityInfo>
{
	public Mk23StandardMagazine() : base( 12 ) { }

	public EntityInfo Item => new EntityInfo
	{
		Display = "12-Round Magazine",
		Brief = "For MK23",
		Icon = "swipe_up",
	};

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "weapons/mk23/espionage_mk23_12r_mag.vmdl" );
		Tags.Add( "pickup" );

		PhysicsEnabled = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}
}
