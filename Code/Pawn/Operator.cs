using Sandbox;

namespace Woosh.Espionage;

public sealed class Operator : Pawn
{
	public Entity Holding => Components.Get<CarriableHandler>().Active;
	public IEntityInventory Inventory => Components.Get<IEntityInventory>();
	public CarrySlotHandler Slots => Components.Get<CarrySlotHandler>();

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
		Components.Add( new ViewAnglesPitchLimiter( 70, 75 ) );

		// Camera
		Components.Create<FirstPersonCamera>();

		// States
		Components.Create<ControllablePawnState>();
		Components.Create<FallPawnState>();

		// Inventory
		Components.Create<EntityInventoryContainer>();
		Components.Create<CarriableHandler>();
		Components.Create<CarrySlotHandler>();
		Components.Create<PawnHandsHandler>();

		// Interaction
		Components.Create<InteractionHandler>();
		Components.Create<PushEntityInteraction>();
		Components.Create<UseEntityInteraction>();
		Components.Create<PickupEntityInteraction>();
		Components.Create<EquipEntityInteraction>();
	}

	// Camera Setup

	protected override void PreCameraSetup( ref CameraSetup setup )
	{
		
	}

	protected override void PostCameraSetup( ref CameraSetup setup )
	{
		Components.Get<ViewModelHandlerComponent>().OnMutate( ref setup );
	}
}
