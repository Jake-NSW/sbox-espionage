using System;
using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelSwayEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	private readonly float m_Multiplier;
	private readonly float m_AimMultiplier;

	private float Influence { get; init; } = 0.05f;
	private float ReturnSpeed { get; init; } = 5.0f;
	private float MaxOffsetLength { get; init; } = 16.0f;

	public ViewModelSwayEffect( float multiplier = 1, float aimMultiplier = 0.2f )
	{
		m_Multiplier = multiplier;
		m_AimMultiplier = aimMultiplier;
	}

	private Angles m_Angles;
	private Vector2 m_Velocity;

	void IMutate<CameraSetup>.OnPostSetup( ref CameraSetup setup )
	{
		var rot = setup.Rotation.WithRoll( 0 );

		// Workout how much we've moved since last frame
		var camera = setup.Rotation.Angles();
		var angles = (m_Angles - camera).Normal * 1.5f;
		var vel = m_Velocity = Swing( m_Velocity, new Vector2( angles.yaw, -angles.pitch ), ReturnSpeed, Influence, MaxOffsetLength );
		vel *= MathX.Lerp( m_Multiplier, m_AimMultiplier, setup.Hands.Aim );

		m_Angles = camera;

		// Rotate and lerp the viewmodel
		setup.Hands.Angles *= Rotation.From( vel.y, -vel.x, vel.x * 0.6f );

		// Move the viewmodel to a nice new position
		setup.Hands.Offset += rot * new Vector3( -MathF.Abs( vel.x ) * 0.3f, vel.x * 0.5f, vel.y * 0.5f );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static Vector2 Swing( Vector2 vel, Vector2 angles, float returnSpeed, float influence, float length )
	{
		var swingVelocity = new Vector2( angles.x, angles.y );

		vel -= vel * returnSpeed * Time.Delta;
		vel += swingVelocity * influence;

		if ( vel.Length > length )
			vel = vel.Normal * length;

		return vel;
	}
}
