using Sandbox;
using Sandbox.Utility;

namespace Woosh.Espionage;

public sealed class TemporaryCameraLookAtEffect : ITemporaryCameraEffect
{
	public Vector3 Target { get; }

	public TemporaryCameraLookAtEffect( float time, Vector3 target )
	{
		Target = target;
		m_TimeUntil = time;
	}

	private readonly TimeUntil m_TimeUntil;

	bool ITemporaryCameraEffect.OnPostCameraSetup( ref CameraSetup setup )
	{
		var normal = m_TimeUntil.Fraction;
		var eased = Easing.ExpoOut( normal );
		var curved = eased * (1 - eased);

		var lookAt = Rotation.LookAt( (Target - setup.Position).Normal );
		setup.Rotation = Rotation.Lerp( Rotation.Identity, lookAt, curved );
		setup.Hands.Angles *= Rotation.Lerp( Rotation.Identity, lookAt, curved ).Inverse;

		return !m_TimeUntil;
	}
}
