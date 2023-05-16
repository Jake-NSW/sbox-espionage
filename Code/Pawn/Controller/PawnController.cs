using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public abstract class PawnController : ObservableEntityComponent<Pawn>, ISingletonComponent
{
	protected Vector3 Position
	{
		get => Entity.Position;
		set => Entity.Position = value;
	}

	protected Rotation Rotation
	{
		get => Entity.Rotation;
		set => Entity.Rotation = value;
	}

	protected Vector3 Velocity
	{
		get => Entity.Velocity;
		set => Entity.Velocity = value;
	}

	public virtual void Simulate( IClient cl ) { }
}
