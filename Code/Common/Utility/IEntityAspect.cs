using Sandbox;

namespace Woosh.Espionage;

public interface IEntityAspect<in T> where T : class, IEntity
{
	void ImportFrom( T value, IComponentSystem system );
	void ExportTo( T value, IComponentSystem system );
}
