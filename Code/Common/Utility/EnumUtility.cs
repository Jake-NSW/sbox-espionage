using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

[SkipHotload]
public static class EnumUtility<T> where T : Enum
{
	[SkipHotload] private readonly static T[] s_Values;

	static EnumUtility()
	{
		s_Values = (T[])typeof(T).GetEnumValues();
	}

	public static int Length => s_Values.Length;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
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

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T ValueOf( int index )
	{
		return s_Values[index - 1];
	}
}
