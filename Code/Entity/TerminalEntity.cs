using Sandbox;

namespace Woosh.Espionage;

public sealed class TerminalEntity : AnimatedEntity, IControllable
{
	public bool IsUsable( Entity pawn )
	{
		return true;
	}

	public void Entering( Entity pawn )
	{
		// Transition Camera to some point?
	}

	public void Leaving()
	{
		// Reset Camera
	}

	public bool Simulate( Entity pawn )
	{
		// Check Feed, return true once we're done
		return false;
	}
}
