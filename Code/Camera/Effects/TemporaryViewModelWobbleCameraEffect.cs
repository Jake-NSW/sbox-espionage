using System;
using Sandbox;

namespace Woosh.Espionage;

public sealed class TemporaryViewModelWobbleCameraEffect : ITemporaryCameraEffect
{
	public TemporaryViewModelWobbleCameraEffect( float time, float speed, float amplitude )
	{
		m_TimeUntil = time;
		
		m_Speed = speed;
		m_Amplitude = amplitude;
	}

	private readonly TimeUntil m_TimeUntil;
	private readonly float m_Speed;
	private readonly float m_Amplitude;

	public bool OnPostCameraSetup( ref CameraSetup setup )
	{
		float currentAmplitude = m_Amplitude * (1 - m_TimeUntil.Passed.Clamp(0, 1));
		
		Log.Info(currentAmplitude);

		var wobbleAmount = MathF.Sin( m_TimeUntil.Fraction * 8 ) * currentAmplitude;
		// setup.Hands.Offset += setup.Rotation * new Vector3( 0, wobbleAmount, 0 );
		setup.Hands.Angles *= Rotation.FromRoll( wobbleAmount );
		return !m_TimeUntil;
	}
}
