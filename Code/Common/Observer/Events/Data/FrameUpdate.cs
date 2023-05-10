namespace Woosh.Common;

public readonly struct FrameUpdate : IEventData
{
	public float Delta { get; }

	public FrameUpdate( float delta )
	{
		Delta = delta;
	}
}
