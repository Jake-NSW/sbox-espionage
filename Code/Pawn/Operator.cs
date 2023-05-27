using System.Linq;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class Operator : Pawn, IMutateCameraSetup
{
	public Entity Active => Components.Get<CarriableHandler>().Active;
	public IEntityInventory Inventory => Components.Get<IEntityInventory>();
	public DeployableSlotHandler Slots => Components.Get<DeployableSlotHandler>();

	public override void Spawn()
	{
		base.Spawn();
		
		// UI
		Components.Create<InteractionHudComponent>();
		Components.Create<InventorySlotsHudComponent>();

		// Gameplay
		Components.Create<PawnLeaning>();
		Components.Create<WalkController>();
		Components.Create<ViewModelHandlerComponent>();

		// Interaction
		Components.Create<InteractionHandler>();
		Components.Create<PushEntityInteraction>();
		Components.Create<UseEntityInteraction>();
		
		// Controllable
		Components.Create<ControllableEntityInteraction>();
		Components.Create<ControllableSimulatedEntityState>();

		// Inventory
		Components.Create<CarriableHandler>();
		Components.Create<InventoryContainer>();
		Components.Add( new DeployableSlotHandler( 3 ) );
		Components.Create<PickupEntityInteraction>();
		Components.Create<EquipEntityInteraction>();
	}

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		(Active as IMutateCameraSetup)?.OnPostCameraSetup( ref setup );

		foreach ( var component in Components.All().OfType<IMutateCameraSetup>() )
		{
			component.OnPostCameraSetup( ref setup );
		}
	}

	protected override void OnPostInputBuild( ref InputContext context )
	{
		context.ViewAngles.pitch = context.ViewAngles.pitch.Clamp( -75, 70 );

		(Active as IMutateInputContext)?.OnPostInputBuild( ref context );
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Machine.Active != null )
			return;

		if ( Input.Pressed( "slot_primary" ) )
		{
			Slots.Deploy( CarrySlot.Front );
		}

		if ( Input.Pressed( "slot_secondary" ) )
		{
			Slots.Deploy( CarrySlot.Back );
		}

		if ( Input.Pressed( "slot_holster" ) )
		{
			Slots.Deploy( CarrySlot.Holster );
		}

		if ( Input.Pressed( "drop" ) )
		{
			Slots.Drop( Slots.Active );
		}
	}
}
