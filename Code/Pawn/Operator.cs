using Sandbox;

namespace Woosh.Espionage;

public sealed class Operator : Pawn
{
	public IEntityInventory Inventory => Components.Get<IEntityInventory>();
	public CarriableHandler Hands => Components.Get<CarriableHandler>();
	public DeployableSlotHandler Slots => Components.Get<DeployableSlotHandler>();

	public override void Spawn()
	{
		base.Spawn();
		
		// Gameplay
		Components.Create<PawnLeaning>();

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

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( "slot_primary" ) )
		{
			Slots.Deploy( 1 );
		}

		if ( Input.Pressed( "slot_secondary" ) )
		{
			Slots.Deploy( 2 );
		}

		if ( Input.Pressed( "slot_holster" ) )
		{
			Slots.Deploy( 3 );
		}

		if ( Input.Pressed( "drop" ) )
		{
			Slots.Drop( Slots.SlotOfEntity( Hands.Active ) );
		}
		
		Components.Get<PawnLeaning>()?.Simulate(cl);
	}
}
