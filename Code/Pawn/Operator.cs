using Sandbox;
using Woosh.Espionage;

namespace Woosh.Espionage;

public sealed class Operator : Pawn
{
	public Entity Active => Components.Get<CarriableHandler>().Active;
	public IEntityInventory Inventory => Components.Get<IEntityInventory>();
	public InventorySlotHandler Slots => Components.Get<InventorySlotHandler>();
	public CarriableHandler Carriable => Components.Get<CarriableHandler>();

	public override void Spawn()
	{
		base.Spawn();

		// UI
		Components.Create<InteractionHudComponent>();
		Components.Create<InventoryNotificationHudComponent>();
		Components.Create<CarriableDeployOverlayHudComponent>();
		Components.Create<OperatorEquipmentHudComponent>();
		Components.Create<AmmoCheckOverlayHudComponent>();
		Components.Create<CrosshairHudComponent>();

		// Gameplay
		Components.Create<PawnLeaningHandler>();
		Components.Create<WalkController>();
		Components.Create<ViewModelHandlerComponent>();
		Components.Create<PawnRagDollEntityState>();
		Components.Create<OperatorHandsHandler>();
		Components.Add( new ViewAnglesPitchLimiter( 70, 75 ) );

		// Camera
		Components.Create<FirstPersonCamera>();

		// Interaction
		Components.Create<InteractionHandler>();
		Components.Create<PushEntityInteraction>();
		Components.Create<UseEntityInteraction>();

		// Controllable
		Components.Create<ControllableEntityInteraction>();
		Components.Create<ControllableEntityState>();

		// Inventory
		Components.Create<CarriableHandler>();
		Components.Create<EntityInventoryContainer>();
		Components.Add( new InventorySlotHandler( EnumUtility<CarrySlot>.Length ) );
		Components.Create<PickupEntityInteraction>();
		Components.Create<EquipEntityInteraction>();
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Machine.InState )
			return;

		for ( var i = 0; i < EnumUtility<CarrySlot>.Length; i++ )
		{
			var value = EnumUtility<CarrySlot>.ValueOf( i + 1 );
			if ( Input.Pressed( value.ToInputAction() ) )
			{
				// If item is active, holster and deploy arms
				if ( Slots.Active - 1 == i )
					Carriable.Holster( false );
				else
					Slots.Deploy( value );
			}
		}

		if ( Input.Pressed( App.Actions.Drop ) )
		{
			Slots.Drop( Slots.Active );
		}
	}

}
