using Sandbox;

namespace Woosh.Espionage;

public interface IEntityInventory : IReadOnlyEntityInventory, IComponent
{
	bool Add( Entity ent );
	void Drop( Entity ent );
}
