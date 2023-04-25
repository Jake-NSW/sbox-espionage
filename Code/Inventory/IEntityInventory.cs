using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Woosh.Espionage;

public interface IReadOnlyEntityInventory
{
	event Action<Entity> Added;
	
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
	public static T Get<T>( this IReadOnlyEntityInventory inv ) where T : Entity
	{
		return inv.All.OfType<T>().FirstOrDefault();
	}
	
	public static bool Contains<T>(this IReadOnlyEntityInventory inv) where T : Entity
	{
		return inv.All.OfType<T>().Any();
	}
}
