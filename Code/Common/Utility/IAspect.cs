using Sandbox;

namespace Woosh.Common;

public interface IAspect<in T> where T : class, IEntity
{
	void Fill( T value );
}
