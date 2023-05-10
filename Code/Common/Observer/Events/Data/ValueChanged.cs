namespace Woosh.Common;

public readonly struct ValueChanged<T> : IEventData
{
	public ValueChanged( T value, T old )
	{
		New = value;
		Old = old;
	}

	public T New { get; }
	public T Old { get; }
}
