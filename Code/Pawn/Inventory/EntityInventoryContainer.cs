using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class EntityInventoryContainer : ObservableEntityComponent, IEntityInventory, ISingletonComponent, INetworkSerializer
{
	public IEnumerable<Entity> All => n_Entities;

	private HashSet<Entity> n_Entities;

	public EntityInventoryContainer()
	{
		n_Entities = new HashSet<Entity>();
	}

	public bool Add( Entity ent )
	{
		Game.AssertServer();

		// If we dont own this and if we cant carry it, ignore
		if ( Contains( ent ) || ent.Owner != null )
		{
			Log.Error( $"Can't pickup item {ent}" );
			return false;
		}

		// Add to Bucket
		n_Entities.Add( ent );

		// Apply things to Entity
		ent.SetParent( Entity, "weapon_attach", Transform.Zero );
		ent.Owner = Entity;
		ent.EnableDrawing = false;

		// Callback for Entity
		(ent as IPickup)?.OnPickup( Entity );

		RunInventoryAdded( ent );

		m_NetHash ^= ent.GetHashCode();
		WriteNetworkData();

		return true;
	}

	public void Drop( Entity ent )
	{
		Game.AssertServer();

		if ( ent.Owner != Entity )
		{
			Log.Error( $"Failed to drop - {ent}, Owner didn't match item Owner" );
			return;
		}

		if ( !Contains( ent ) )
		{
			Log.Error( $"Failed to drop - {ent}, Entity wasn't in bucket" );
			return;
		}

		// Remove from Bucket
		n_Entities.Add( ent );

		// Apply things to Entity
		ent.SetParent( null );
		ent.EnableDrawing = true;

		RunInventoryRemoved( ent );

		// Execute Callback
		(ent as IPickup)?.OnDrop();

		ent.Owner = null;

		m_NetHash ^= ent.GetHashCode();
		WriteNetworkData();
	}

	public bool Contains( Entity entity )
	{
		return n_Entities.Contains( entity );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private void RunInventoryRemoved( Entity ent ) => Run( new InventoryRemoved( ent ), Propagation.Both );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private void RunInventoryAdded( Entity ent ) => Run( new InventoryAdded( ent ), Propagation.Both );

	// Serialization

	private int m_NetHash;

	void INetworkSerializer.Read( ref NetRead read )
	{
		if ( Entity == null )
			return;

		var hash = read.Read<int>();
		if ( m_NetHash == hash )
			return;

		m_NetHash = hash;

		var length = read.Read<int>();
		if ( length <= 0 )
			return;

		var oldEntities = n_Entities;
		var received = new HashSet<Entity>();

		for ( var i = 0; i < length; i++ )
		{
			var ent = Entity.FindByIndex<Entity>( read.Read<int>() );
			received.Add( ent );
			if ( !oldEntities.Contains( ent ) )
			{
				// New Entity
				RunInventoryAdded( ent );
			}
		}

		// Remove old entities
		foreach ( var oldEnt in oldEntities )
		{
			if ( !received.Contains( oldEnt ) )
			{
				// Removed
				RunInventoryRemoved( oldEnt );
			}
		}

		n_Entities = received;
	}

	void INetworkSerializer.Write( NetWrite write )
	{
		write.Write( m_NetHash );
		write.Write( n_Entities.Count );
		
		foreach ( var entity in n_Entities )
		{
			write.Write( entity.NetworkIdent );
		}
	}
}
