using System;
using System.Collections.Generic;
using Sandbox;

namespace Woosh.Espionage;

public class DeployableSlotHandler : EntityComponent, ISingletonComponent, INetworkSerializer
{
	private IEntityInventory Inventory => Entity.Components.Get<IEntityInventory>();
	private CarriableHandler Handler => Entity.Components.Get<CarriableHandler>();

	public DeployableSlotHandler() => Game.AssertClient();

	public DeployableSlotHandler( int slots )
	{
		Game.AssertServer();
		n_Slots = new Entity[slots];
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Inventory.Added += OnInventoryAdded;
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		Inventory.Added -= OnInventoryAdded;
	}

	private void OnInventoryAdded( Entity ent )
	{
		if ( ent is not ISlotted slotted )
		{
			// Don't do anything, as we can't slot...
			return;
		}

		var wasActive = Active == slotted.Slot;

		Drop( slotted.Slot );
		Assign( slotted.Slot, ent );

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
		slot -= 1;

		Game.AssertServer();
		n_Slots[slot] = ent;
		WriteNetworkData();
	}

	public int SlotOfEntity( Entity entity )
	{
		for ( int i = 0; i < n_Slots.Length; i++ )
		{
			if ( entity == n_Slots[i] )
				return i + 1;
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

		var entity = n_Slots[slot - 1];
		if ( SlotOfEntity( Handler.Active ) == slot )
		{
			Handler.Holster( true, ent => ent.Components.Get<IEntityInventory>().Drop( entity ) );
			return;
		}

		Inventory.Drop( entity );
	}

	public void Holster()
	{
		if ( Game.IsClient )
			return;

		Handler.Holster( false );
	}

	// Networking

	void INetworkSerializer.Read( ref NetRead read )
	{
		var length = read.Read<int>();
		n_Slots = new Entity[length];

		for ( var i = 0; i < length; i++ )
		{
			var id = read.Read<int>();
			n_Slots[i] = id == 0 ? null : Entity.FindByIndex<Entity>( id );
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
	private static class EnumValues<TSlot> where TSlot : Enum
	{
		private readonly static TSlot[] s_Values;

		public static int IndexOf( TSlot input )
		{
			var compare = EqualityComparer<TSlot>.Default;

			for ( var i = 0; i < s_Values.Length; i++ )
			{
				if ( compare.Equals( s_Values[i], input ) )
					return i + 1;
			}

			throw new InvalidOperationException( "What the fuck" );
		}

		static EnumValues()
		{
			s_Values = (TSlot[])typeof(TSlot).GetEnumValues();
		}
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
