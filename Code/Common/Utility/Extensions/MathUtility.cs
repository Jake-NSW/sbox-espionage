using System;
using System.Runtime.CompilerServices;

namespace Woosh.Common;

public static class MathUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Max( this float value, float max )
	{
		return MathF.Max( value, max );
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Round( this float value, int digits )
	{
		return MathF.Round( value, digits );
	}
	
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Min( this float value, float min )
	{
		return MathF.Min( value, min );
	}
}
