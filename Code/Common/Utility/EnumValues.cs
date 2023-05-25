using System;
using System.Collections.Generic;
using Sandbox;

namespace Woosh.Common;

public static class EnumValues<T> where T : Enum
{
	[SkipHotload] private readonly static T[] s_Values;

	static EnumValues()
	{
		s_Values = (T[])typeof(T).GetEnumValues();
	}

	public static int Length => s_Values.Length;

	public static int IndexOf( T input )
	{
		var compare = EqualityComparer<T>.Default;

		for ( var i = 0; i < s_Values.Length; i++ )
		{
			if ( compare.Equals( s_Values[i], input ) )
			{
				return i + 1;
			}
		}

		throw new InvalidOperationException();
	}
}
