using System;

namespace Woosh.Espionage;

public sealed class Factory<T>
{
	public static Func<T> From( string value )
	{
		return default;
	}

	public static Func<T> From( Type value )
	{
		return default;
	}

	public static Func<T> From<TType>()
	{
		return default;
	}
}
