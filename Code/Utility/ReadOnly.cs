namespace Woosh.Espionage;

public readonly struct ReadOnly<T> where T : struct
{
	public ReadOnly( T value )
	{
		Read = value;
	}

	public T Read { get; }

	public static implicit operator T( ReadOnly<T> v ) => v.Read;
}
