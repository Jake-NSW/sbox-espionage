using System;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

public static class MathUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Damp( ref float value, float to, float smoothing, float delta ) => value = Damp( value, to, smoothing, delta );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Damp( this float value, float to, float smoothing, float delta )
	{
		var blend = MathF.Pow( 0.5f, delta * smoothing );
		return MathX.Lerp( to, value, blend );

		// var blend = 1 - MathF.Pow( 0.5f, delta * smoothing );
		// return MathX.Lerp( value, to, blend );

		// return MathX.Lerp( value, to, 1 - MathF.Exp( -smoothing * delta ) );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Damp( ref Vector3 value, Vector3 to, float smoothing, float delta ) => value = Damp( value, to, smoothing, delta );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3 Damp( this Vector3 value, Vector3 to, float smoothing, float delta )
	{
		return new Vector3(
			x: value.x.Damp( to.x, smoothing, delta ),
			y: value.y.Damp( to.y, smoothing, delta ),
			z: value.z.Damp( to.z, smoothing, delta )
		);
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Damp( ref Rotation value, Rotation to, float smoothing, float delta ) => value = Damp( value, to, smoothing, delta );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotation Damp( this Rotation value, Rotation to, float smoothing, float delta )
	{
		return Rotation.Slerp( to, value, MathF.Pow( 0.5f, delta * smoothing ) );

		// return Rotation.Lerp( value, to, 1 - MathF.Pow( 0.5f, delta * smoothing ) );

		// return Rotation.Lerp( value, to, 1 - MathF.Exp( -smoothing * delta ) );
	}

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

	// Quaternion

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotation WithPitch( this Rotation rotation, float pitch )
	{
		var rot = rotation * Rotation.FromPitch( rotation.Pitch() ).Inverse;
		rot *= Rotation.FromPitch( pitch );
		return rot;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotation WithYaw( this Rotation rotation, float pitch )
	{
		var rot = rotation * Rotation.FromYaw( rotation.Yaw() ).Inverse;
		rot *= Rotation.FromYaw( pitch );
		return rot;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Range( this float value )
	{
		return Game.Random.Float( -value, value );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotation WithRoll( this Rotation rotation, float pitch )
	{
		var rot = rotation * Rotation.FromRoll( rotation.Roll() ).Inverse;
		rot *= Rotation.FromRoll( pitch );
		return rot;
	}

	// Vector2

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2 MoveTowards( this Vector2 from, Vector2 target, float t )
	{
		return new Vector2( from.x.Approach( target.x, t ), from.y.Approach( target.y, t ) );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2 SmoothDamp( this Vector2 current, Vector2 target, ref Vector2 velocity, float smooth, float delta )
	{
		var xVel = velocity.x;
		var yVel = velocity.y;

		var x = MathX.SmoothDamp( current.x, target.x, ref xVel, smooth, delta );
		var y = MathX.SmoothDamp( current.y, target.y, ref yVel, smooth, delta );

		velocity = new Vector2( xVel, yVel );
		return new Vector2( x, y );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2 ClampMagnitude( this Vector2 value, float maxLength )
	{
		if ( value.LengthSquared > maxLength * maxLength )
		{
			return value.Normal * maxLength;
		}

		return value;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2 WithLength( this Vector2 vector, float length )
	{
		return vector.Normal * length;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2 WithLength( this Vector2 vector, Vector2 length )
	{
		return vector.Normal * length;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float DistanceTo( this Vector2 vector, Vector2 other )
	{
		return (other - vector).Length;
	}

	// Vector3 

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3 Spring( Vector3 from, Vector3 to, float time )
	{
		return new Vector3( Spring( from.x, to.x, time ), Spring( from.y, to.y, time ), Spring( from.z, to.z, time ) );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3 SmoothDamp( this Vector3 current, Vector3 target, ref Vector3 velocity, float smooth, float delta )
	{
		var xVel = velocity.x;
		var yVel = velocity.y;
		var zVel = velocity.z;

		var x = MathX.SmoothDamp( current.x, target.x, ref xVel, smooth, delta );
		var y = MathX.SmoothDamp( current.y, target.y, ref yVel, smooth, delta );
		var z = MathX.SmoothDamp( current.z, target.z, ref zVel, smooth, delta );

		velocity = new Vector3( xVel, yVel, zVel );
		return new Vector3( x, y, z );
	}

	// Int

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static int Approach( this int f, int target, int delta )
	{
		if ( f > target )
		{
			f -= delta;
			if ( f < target )
				return target;
		}
		else
		{
			f += delta;
			if ( f > target )
				return target;
		}

		return f;
	}
}
