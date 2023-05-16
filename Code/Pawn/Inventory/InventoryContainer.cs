using System.Collections.Generic;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public partial class InventoryContainer : ObservableEntityComponent<Pawn>, IEntityInventory, ISingletonComponent
{
	public IEnumerable<Entity> All => n_Bucket;

	[Net, Local] private IList<Entity> n_Bucket { get; set; }

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
		n_Bucket.Add( ent );

		// Apply things to Entity
		ent.SetParent( Entity, "weapon_attach", Transform.Zero );
		ent.Owner = Entity;
		ent.EnableDrawing = false;

		// Callback for Entity
		(ent as IPickup)?.OnPickup( Entity );

		Events.Run( new InventoryAdded( ent ) );
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
		n_Bucket.Remove( ent );

		// Apply things to Entity
		ent.SetParent( null );
		ent.EnableDrawing = true;

		Events.Run( new InventoryRemoved( ent ) );

		// Execute Callback
		(ent as IPickup)?.OnDrop();

		ent.Owner = null;
	}

	public bool Contains( Entity entity )
	{
		return n_Bucket.Contains( entity );
	}
}

