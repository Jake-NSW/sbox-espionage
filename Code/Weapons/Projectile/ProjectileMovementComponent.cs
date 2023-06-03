using System;
using Sandbox;
using Sandbox.Internal.Globals;

namespace Woosh.Espionage;

public readonly ref struct ProjectMovementHelper
{
	public float Force { get; }
	public float Mass { get; }

	public ProjectMovementHelper( float force, float mass )
	{
		Force = force;
		Mass = mass;
	}
	
	public Vector3 AtTime( TimeSince since, Vector3 start, Vector3 direction )
	{
		var velocity = Force / Mass * Game.TickInterval;

		var position = start + direction * velocity * since;
		position += Game.PhysicsWorld.Gravity / 2 * MathF.Pow( since, 2 );

		return position;
	}

	public Vector3[] Sample( Vector3 start, Vector3 direction, float duration, float interval )
	{
		var steps = (int)(duration / interval);

		var velocity = Force / Mass * Game.TickInterval;
		var samples = new Vector3[steps];

		for ( var i = 0; i < steps; i++ )
		{
			var position = start + direction * velocity * i * interval;
			position += Game.PhysicsWorld.Gravity / 2 * MathF.Pow( i * interval, 2 );

			samples[i] = position;
		}

		return samples;
	}
}

public static class ProjectMovementHelperExtensions
{
	public static void Projectile( this DebugOverlay overlay, ProjectMovementHelper moveHelper, Vector3 start, Vector3 direction, float time, float duration = 1, float interval = 0.01f )
	{
		var samples = moveHelper.Sample( start, direction, duration, interval );

		for ( int i = 1; i < samples.Length; i++ )
		{
			overlay.Sphere( samples[i - 1], 0.2f, Color.Red, duration: time );
			overlay.Line( samples[i - 1], samples[i], Color.Black, duration: time );

			var ray = Trace.Ray( samples[i - 1], samples[i] )
				.Ignore( Game.LocalPawn )
				.Size( 1 )
				.Run();

			if ( ray.Hit )
			{
				overlay.Sphere( ray.EndPosition, 4f, Color.Blue, duration: time );
				break;
			}
		}
	}
}
