using Sandbox;

namespace Woosh.Espionage;

public sealed partial class EntityLifetimeAttachComponent : EntityComponent
{
	[Net] private Entity n_Target { get; set; }

	public EntityLifetimeAttachComponent() { }

	public EntityLifetimeAttachComponent( Entity nTarget )
	{
		n_Target = nTarget;
	}

	protected override void OnDeactivate()
	{
		n_Target.Delete();
	}
}
