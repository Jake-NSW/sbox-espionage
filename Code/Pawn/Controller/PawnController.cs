using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public abstract class PawnController : ObservableEntityComponent<Pawn>, ISingletonComponent, ISimulated
{
	protected Vector3 Position
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get => Entity.Position;
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		set => Entity.Position = value;
	}

	protected Rotation Rotation
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get => Entity.Rotation;
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		set => Entity.Rotation = value;
	}

	protected Vector3 Velocity
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get => Entity.Velocity;
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		set => Entity.Velocity = value;
	}

	public virtual void Simulate( IClient cl ) { }
}
