using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

/// <summary>
/// A set of utility methods for working with enums. This is a generic class where the generic type is the enum type.
/// <typeparam name="T"> The enum we want utilities for. </typeparam>
/// </summary>
[SkipHotload]
public static class EnumUtility<T> where T : Enum
{
	[SkipHotload] private readonly static T[] s_Values;

	static EnumUtility()
	{
		s_Values = (T[])typeof(T).GetEnumValues();
	}

	/// <summary>
	/// The amount of items / values found within the enum.
	/// </summary>
	public static int Length => s_Values.Length;

	/// <summary>
	/// Returns the index of an enum value. This is useful for storing enums in a database as an integer. This is a 1-based index.
	/// </summary>
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

	/// <summary>
	/// Returns the enum value at the given index. This is useful for storing enums in a database as an integer. This is a 1-based index.
	/// </summary>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T ValueOf( int index )
	{
		return s_Values[index - 1];
	}
}
