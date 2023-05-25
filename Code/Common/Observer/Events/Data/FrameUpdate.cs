using Woosh.Signals;

namespace Woosh.Common;

public readonly struct FrameUpdate : ISignal
{
	public float Delta { get; }

	public FrameUpdate( float delta )
	{
		Delta = delta;
	}
}
