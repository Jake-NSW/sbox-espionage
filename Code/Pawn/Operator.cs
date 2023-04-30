namespace Woosh.Espionage;

public sealed class Operator : Pawn
{
	public IEntityInventory Inventory => Components.Get<IEntityInventory>();
	public CarriableHandler Hands => Components.Get<CarriableHandler>();
	public DeployableSlotHandler Slots => Components.Get<DeployableSlotHandler>();

	public override void Spawn()
	{
		base.Spawn();

		// Interaction
		Components.Create<InteractionHandler>();
		Components.Create<PushEntityInteraction>();
		Components.Create<UseEntityInteraction>();
		Components.Create<PickupEntityInteraction>();

		// Inventory
		Components.Create<CarriableHandler>();
		Components.Create<InventoryContainer>();
		Components.Add( new DeployableSlotHandler( 3, Inventory, Hands ) );
	}
}
