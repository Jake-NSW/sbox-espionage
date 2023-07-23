using Sandbox;

namespace Woosh.Espionage;

public interface IControllable : IEntity
{
	bool IsUsable( Entity pawn );

	void Entering( Entity pawn );
	void Leaving();

	bool Simulate( Entity pawn );
}
