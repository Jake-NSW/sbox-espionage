using Sandbox;

namespace Woosh.Espionage;

public sealed class PickupEntityInteraction : EntityComponent, IEntityInteraction
{
	public void Simulate( TraceResult hovering, IClient client )
	{
		if ( Game.IsClient )
			return;

		if ( hovering.Entity is not IPickup )
			return;

		using var _ = Prediction.Off();

		if ( Input.Pressed( InputButton.Use ) )
		{
			Entity.Components.Get<IEntityInventory>().Add( hovering.Entity );
		}
	}
}
