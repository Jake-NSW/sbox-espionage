using System;
using System.Collections.Generic;

namespace Woosh.Espionage;

public static class EnumValues<T> where T : Enum
{
	private readonly static T[] s_Values;

	public static int Length => s_Values.Length;

	public static int IndexOf( T input )
	{
		var compare = EqualityComparer<T>.Default;

		for ( var i = 0; i < s_Values.Length; i++ )
		{
			if ( compare.Equals( s_Values[i], input ) )
				return i + 1;
		}

		throw new InvalidOperationException();
	}

	static EnumValues()
	{
		s_Values = (T[])typeof(T).GetEnumValues();
	}
}
