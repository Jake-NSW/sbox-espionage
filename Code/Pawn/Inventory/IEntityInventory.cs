using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Woosh.Espionage;

public interface IReadOnlyEntityInventory
{
	IEnumerable<Entity> All { get; }
	bool Contains( Entity entity );
}

public interface IEntityInventory : IReadOnlyEntityInventory, IComponent
{
	bool Add( Entity ent );
	void Drop( Entity ent );
}

public static class EntityInventoryUtility
{
	public static T GetAny<T>( this IReadOnlyEntityInventory inv ) where T : Entity
	{
		return inv.All.OfType<T>().FirstOrDefault();
	}

	public static T Create<T>( this IEntityInventory inventory ) where T : Entity, new()
	{
		var item = new T();
		inventory.Add( item );
		return item;
	}

	public static T DropAny<T>( this IEntityInventory inventory ) where T : Entity
	{
		var item = inventory.GetAny<T>();
		inventory.Drop(item);
		return item;
	}

	public static bool ContainsAny<T>( this IReadOnlyEntityInventory inv ) where T : Entity
	{
		return inv.All.OfType<T>().Any();
	}
}
