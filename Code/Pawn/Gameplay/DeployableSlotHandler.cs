using System.Collections.Generic;
using Sandbox;

namespace Woosh.Espionage;

public sealed class DeployableSlotHandler : EntityComponent, ISingletonComponent, INetworkSerializer
{
	private IEntityInventory Inventory { get; }
	private CarriableHandler Handler { get; }

	public DeployableSlotHandler()
	{
		Game.AssertClient();
	}

	public DeployableSlotHandler( int slots, IEntityInventory inventory, CarriableHandler handler )
	{
		Game.AssertServer();

		n_Slots = new Entity[slots];
		Inventory = inventory;
		Handler = handler;
	}

	// Slots

	public IReadOnlyList<Entity> Slots => n_Slots;
	private Entity[] n_Slots;

	public void Assign( int slot, Entity ent )
	{
		slot -= 1;

		Game.AssertServer();
		n_Slots[slot] = ent;
		WriteNetworkData();
	}

	public void Deploy( int slot, DrawTime? timing = null)
	{
		if ( Game.IsClient )
			return;

		slot -= 1;

		// Deploy to Slot
		if ( slot > n_Slots.Length )
			return;

		var ent = n_Slots[slot];
		if ( !Inventory.Contains( ent ) )
			return;

		Handler.Deploy( ent, timing );
	}

	public void Drop( int slot )
	{
		if ( Game.IsClient )
			return;
		
		slot -= 1;
		Handler.Holster( true, ent => ent.Components.Get<IEntityInventory>().Drop( n_Slots[slot] ) );
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
