using System.Collections.Generic;
using Sandbox;

namespace Woosh.Espionage;

public partial class InventoryContainer : EntityComponent, IEntityInventory
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

		// Callback for Entity
		(ent as IPickup)?.OnPickup( Entity );

		return true;
	}

	public void Drop( Entity ent )
	{
		Game.AssertServer();

		if ( !Contains( ent ) || ent.Owner != Entity )
			return;

		// Remove from Bucket
		n_Bucket.Remove( ent );

		// Apply things to Entity
		ent.SetParent( null );
		ent.EnableDrawing = true;

		// Execute Callback
		(ent as IPickup)?.OnDrop();

		ent.Owner = null;
	}

	public bool Contains( Entity entity )
	{
		return n_Bucket.Contains( entity );
	}
}
