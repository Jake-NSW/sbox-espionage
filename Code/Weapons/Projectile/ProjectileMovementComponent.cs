using System;
using Sandbox;

namespace Woosh.Espionage;

public readonly ref struct ProjectMovementHelper
{
	public Vector3[] Sample( Vector3 start, Vector3 direction, float force, float mass, float duration, float interval )
	{
		var steps = (int)(duration / interval);

		var velocity = force / mass * Game.TickInterval;
		var samples = new Vector3[steps];

		for ( int i = 0; i < steps; i++ )
		{
			var position = start + direction * velocity * i * interval;
			position += Game.PhysicsWorld.Gravity / 2 * MathF.Pow( i * interval, 2 );

			samples[i] = position;
		}

		return samples;
	}
}
