using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class Operator : Pawn, IMutate<CameraSetup>, IMutate<InputContext>
{
	public Entity Active => Components.Get<CarriableHandler>().Active;
	public IEntityInventory Inventory => Components.Get<IEntityInventory>();
	public DeployableSlotHandler Slots => Components.Get<DeployableSlotHandler>();

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
		Components.Create<PawnLeaning>();
		Components.Create<WalkController>();
		Components.Create<ViewModelHandlerComponent>();
		Components.Create<PawnRagDollSimulatedEntityState>();

		// Camera
		Components.Create<FirstPersonEntityCamera>();

		// Interaction
		Components.Create<InteractionHandler>();
		Components.Create<PushEntityInteraction>();
		Components.Create<UseEntityInteraction>();

		// Controllable
		Components.Create<ControllableEntityInteraction>();
		Components.Create<ControllableSimulatedEntityState>();

		// Inventory
		Components.Create<CarriableHandler>();
		Components.Create<EntityInventoryContainer>();
		Components.Add( new DeployableSlotHandler( 5 ) );
		Components.Create<PickupEntityInteraction>();
		Components.Create<EquipEntityInteraction>();
	}

	void IMutate<CameraSetup>.OnPostSetup( ref CameraSetup setup )
	{
		(Active as IMutate<CameraSetup>)?.OnPostSetup( ref setup );

		foreach ( var component in Components.All() )
		{
			if ( component is IMutate<CameraSetup> cast )
				cast.OnPostSetup( ref setup );
		}
	}

	void IMutate<InputContext>.OnPostSetup( ref InputContext setup )
	{
		setup.ViewAngles.pitch = setup.ViewAngles.pitch.Clamp( -75, 70 );
		(Active as IMutate<InputContext>)?.OnPostSetup( ref setup );

		foreach ( var component in Components.All() )
		{
			if ( component is IMutate<InputContext> cast )
				cast.OnPostSetup( ref setup );
		}
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Machine.Active != null )
			return;

		for ( var i = 0; i < EnumValues<CarrySlot>.Length; i++ )
		{
			var value = EnumValues<CarrySlot>.ValueOf( i + 1 );
			if ( Input.Pressed( value.ToInputAction() ) )
				Slots.Deploy( value );
		}

		if ( Input.Pressed( "drop" ) )
		{
			Slots.Drop( Slots.Active );
		}
	}

}
