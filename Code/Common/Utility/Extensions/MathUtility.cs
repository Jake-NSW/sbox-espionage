using System;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public static class MathUtility
{
	// Float

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Abs( this float value )
	{
		return MathF.Abs( value );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Max( this float value, float max )
	{
		return MathF.Max( value, max );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Round( this float value, int digits )
	{
		return MathF.Round( value, digits );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Min( this float value, float min )
	{
		return MathF.Min( value, min );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Spring( float from, float to, float time )
	{
		time = time.Clamp( 0f, 1f );
		time = (MathF.Sin( time * MathF.PI * (.2f + 2.5f * time * time * time) ) * MathF.Pow( 1f - time, 2.2f ) + time) * (1f + (1.2f * (1f - time)));
		return from + (to - from) * time;
	}

	// Vector3 

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3 Spring( Vector3 from, Vector3 to, float time )
	{
		return new Vector3( Spring( from.x, to.x, time ), Spring( from.y, to.y, time ), Spring( from.z, to.z, time ) );
	}
}
