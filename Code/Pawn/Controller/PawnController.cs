using Sandbox;

namespace Woosh.Espionage;

public abstract class PawnController : EntityComponent, ISingletonComponent
{
	public virtual void Simulate( IClient cl ) { }
}
