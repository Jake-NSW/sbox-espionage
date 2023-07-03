using System.Collections.Generic;
using Sandbox;

namespace Woosh.Espionage;

public interface IReadOnlyEntityInventory
{
	IEnumerable<Entity> All { get; }
	bool Contains( Entity entity );
}
