using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class CarrySlotHandler : InventorySlotHandler<CarrySlot>
{
	[Listen]
	private void Simulate( Event<SimulateSnapshot> evt )
	{
		for ( var i = 0; i < EnumUtility<CarrySlot>.Length; i++ )
		{
			var value = EnumUtility<CarrySlot>.ValueOf( i + 1 );
			if ( Input.Pressed( value.ToInputAction() ) )
			{
				// If item is active, holster and deploy arms
				if ( Active - 1 == i )
					Handler.Holster( false );
				else
					Deploy( value );
			}
		}

		if ( Input.Pressed( App.Actions.Drop ) )
		{
			Drop( Active );
		}
	}
}

public abstract class InventorySlotHandler<T> : ObservableEntityComponent<Pawn>, ISingletonComponent, INetworkSerializer where T : Enum
{
	protected InventorySlotHandler()
	{
		n_Slots = new Entity[EnumUtility<T>.Length];
	}

	protected override void OnAutoRegister()
	{
		base.OnAutoRegister();

		if ( Game.IsServer )
		{
			Register<InventoryAdded>( OnInventoryAdded );
			Register<InventoryRemoved>( OnInventoryRemoved );
		}

		Register<DeployingEntity>(
			evt =>
			{
				var ent = evt.Signal.Entity;
				var slot = SlotOfEntity( ent );
				if ( slot != -1 )
					Run( new SlotDeploying( slot, ent ) );
			}
		);

		Register<HolsteredEntity>(
			evt =>
			{
				var ent = evt.Signal.Entity;
				var slot = SlotOfEntity( ent );

				if ( slot != -1 )
					Run( new SlotHolstered( slot ) );
			}
		);
	}

	protected IEntityInventory Inventory => this.Get<IEntityInventory>();
	protected CarriableHandler Handler => this.Get<CarriableHandler>();

	// Injected Inventory Logic

	private void OnInventoryRemoved( Event<InventoryRemoved> evt )
	{
		var item = evt.Signal.Item;
		var slot = SlotOfEntity( item );

		if ( slot == -1 )
			return;

		if ( Active - 1 == slot )
			Handler.Deploy( null );

		Assign( slot, null );
	}

	private void OnInventoryAdded( Event<InventoryAdded> evt )
	{
		var item = evt.Signal.Item;

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

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Assign<TEntity>( TEntity ent ) where TEntity : Entity, ISlotted => Assign( ent.Slot, ent );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Assign<TSlot>( TSlot slot, Entity ent ) where TSlot : Enum => Assign( EnumUtility<TSlot>.IndexOf( slot ), ent );

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

		for ( var i = 0; i < n_Slots.Length; i++ )
		{
			if ( entity == n_Slots[i] )
			{
				return i + 1;
			}
		}

		return -1;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Deploy( T slot, DrawTime? time = null ) => Deploy( EnumUtility<T>.IndexOf( slot ), time );

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
		Handler.Deploy( ent, timing );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Drop( T slot ) => Drop( EnumUtility<T>.IndexOf( slot ) );

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
						Log.Error( "Inventory didnt contain item when dropping!" );
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
