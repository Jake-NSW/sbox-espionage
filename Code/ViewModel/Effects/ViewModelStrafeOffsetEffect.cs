using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelStrafeOffsetEffect : IViewModelEffect
{
	public float Damping { get; init; } = 6;
	public float AxisMultiplier { get; init; } = 10;
	public float RollMultiplier { get; init; } = 1;

	public float AngleClamp { get; init; } = 20;

	private float m_LastStrafeOffset;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var rot = setup.Rotation;
		m_LastStrafeOffset = m_LastStrafeOffset.LerpTo( -setup.Transform.NormalToLocal( setup.Hands.Velocity ).y / 180, Damping * Time.Delta );
		m_LastStrafeOffset = m_LastStrafeOffset.Clamp( -10, 10 );

		setup.Hands.Angles *= Rotation.From( 0, 0, m_LastStrafeOffset * AxisMultiplier );
		// setup.Hands.Angles =  (setup.Rotation * setup.Hands.Angles).Clamp( setup.Rotation, AngleClamp);
		
		// setup.Position += rot.Left * (m_LastStrafeOffset / 4 * RollMultiplier);
	}
}
