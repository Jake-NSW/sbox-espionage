using System;
using System.Runtime.CompilerServices;

namespace Woosh.Espionage;

public static class EnumUtility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Index<T>( this T @enum ) where T : Enum
	{
		return EnumValues<T>.IndexOf( @enum );
	}
}
