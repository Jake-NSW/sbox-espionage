using System;
using Sandbox;

namespace Woosh.Espionage;

public readonly ref struct ProjectileMovementHelper
{
	public float Force { get; }
	public float Mass { get; }

	public ProjectileMovementHelper( ProjectileDetails details )
	{
		Force = details.Force;
		Mass = details.Mass;
	}

	public ProjectileMovementHelper( float force, float mass )
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
