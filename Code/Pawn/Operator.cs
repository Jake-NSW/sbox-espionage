namespace Woosh.Espionage;

public sealed class Operator : Pawn
{
	public IEntityInventory Inventory => Components.Get<IEntityInventory>();
	public CarrySlotHandler Slots => Components.Get<CarrySlotHandler>();

	public override void Spawn()
	{
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
		Components.Create<PawnFallEntityState>();
		Components.Create<PawnHandsHandler>();
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
		Components.Create<CarrySlotHandler>();
		Components.Create<PickupEntityInteraction>();
		Components.Create<EquipEntityInteraction>();
	}
}
