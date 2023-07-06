using System;
using System.Collections.Generic;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class DeployableSlotHandler : ObservableEntityComponent<Pawn>, ISingletonComponent, INetworkSerializer
{
	public DeployableSlotHandler()
	{
		Game.AssertClient();
	}

	public DeployableSlotHandler( int slots )
	{
		Game.AssertServer();
		n_Slots = new Entity[slots];
	}

	protected override void OnAutoRegister()
	{
		base.OnAutoRegister();

		Register<DeployingEntity>(
			evt =>
			{
				var ent = evt.Data.Entity;
				var slot = SlotOfEntity( ent );

				if ( slot != -1 )
					Run( new SlotDeploying( slot, ent ) );
			}
		);

		if ( Game.IsServer )
		{
			Register<InventoryAdded>( OnInventoryAdded );
			Register<InventoryRemoved>( OnInventoryRemoved );
		}
	}

	private IEntityInventory Inventory => this.Get<IEntityInventory>();
	private CarriableHandler Handler => this.Get<CarriableHandler>();

	private void OnInventoryRemoved( Event<InventoryRemoved> evt )
	{
		var item = evt.Data.Item;
		var slot = SlotOfEntity( item );

		if ( slot == -1 )
			return;

		if ( Active - 1 == slot )
			Handler.Deploy( null );

		Assign( slot, null );
	}

	private void OnInventoryAdded( Event<InventoryAdded> evt )
	{
		var item = evt.Data.Item;

		if ( item is not ISlotted slotted )
		{
			// Don't do anything, as we can't slot...
			return;
		}

		var wasActive = Active == slotted.Slot;

		Drop( slotted.Slot );
		Assign( slotted.Slot, item );

		if ( wasActive )
		{
			Deploy( slotted.Slot );
		}
	}

	// Slots

	public int Active => SlotOfEntity( Handler.Active );

	public IReadOnlyList<Entity> Slots => n_Slots;
	private Entity[] n_Slots;

	public void Assign( int slot, Entity ent )
	{
		Game.AssertServer();

		slot -= 1;
		n_Slots[slot] = ent;
		Events.Run( new SlotAssigned( slot + 1, ent ) );

		WriteNetworkData();
	}

	public int SlotOfEntity( Entity entity )
	{
		if ( entity == null )
			return -1;

		for ( int i = 0; i < n_Slots.Length; i++ )
		{
			if ( entity == n_Slots[i] )
			{
				return i + 1;
			}
		}

		return -1;
	}

	public void Deploy( int slot, DrawTime? timing = null )
	{
		if ( Game.IsClient )
			return;

		if ( slot < 0 )
			return;

		slot -= 1;

		// Deploy to Slot
		if ( slot > n_Slots.Length )
			return;

		var ent = n_Slots[slot];

		if ( !Inventory.Contains( ent ) )
		{
			Assign( slot + 1, null );
			Handler.Deploy( null );
			return;
		}

		Handler.Deploy( ent, timing );
	}

	public void Drop( int slot )
	{
		if ( Game.IsClient )
			return;

		if ( slot < 0 )
			return;

		var ent = n_Slots[slot - 1];

		if ( ent == null )
			return;

		if ( Active == slot )
		{
			Handler.Holster(
				true, pawn =>
				{
					var inventory = pawn.Components.Get<IEntityInventory>();
					if ( inventory.Contains( ent ) )
						inventory.Drop( ent );
					else
						Log.Error("Inventory didnt contain item when dropping!");
				}
			);
			return;
		}

		Inventory.Drop( ent );
	}

	// Networking

	void INetworkSerializer.Read( ref NetRead read )
	{
		var length = read.Read<int>();

		// Entity is null sometimes when reading?
		if ( length <= 0 || Entity == null )
			return;

		n_Slots ??= new Entity[length];

		for ( var i = 0; i < length; i++ )
		{
			var id = read.Read<int>();
			var ent = id == 0 ? null : global::Sandbox.Entity.FindByIndex<Entity>( id );

			if ( n_Slots[i] != ent )
				Events?.Run( new SlotAssigned( i + 1, ent ) );

			n_Slots[i] = ent;
		}
	}

	void INetworkSerializer.Write( NetWrite write )
	{
		if ( n_Slots == null )
		{
			write.Write( 0 );
			return;
		}

		write.Write( n_Slots.Length );
		foreach ( var ent in n_Slots )
		{
			write.Write( ent?.NetworkIdent ?? 0 );
		}
	}
}

public static class DeployableSlotHandlerUtility
{
	public static void Assign<TEntity>( this DeployableSlotHandler handler, TEntity ent ) where TEntity : Entity, ISlotted
	{
		handler.Assign( ent.Slot, ent );
	}

	public static void Assign<TSlot>( this DeployableSlotHandler handler, TSlot slot, Entity ent ) where TSlot : Enum
	{
		handler.Assign( EnumValues<TSlot>.IndexOf( slot ), ent );
	}

	public static void Deploy<TSlot>( this DeployableSlotHandler handler, TSlot slot, DrawTime? time = null ) where TSlot : Enum
	{
		handler.Deploy( EnumValues<TSlot>.IndexOf( slot ), time );
	}

	public static void Drop<TSlot>( this DeployableSlotHandler handler, TSlot slot ) where TSlot : Enum
	{
		handler.Drop( EnumValues<TSlot>.IndexOf( slot ) );
	}
}
