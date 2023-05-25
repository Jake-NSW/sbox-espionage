using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelSwayEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	private readonly float m_Multiplier;
	private readonly float m_AimMultiplier;

	public float Damping { get; set; } = 6;
	public Angles AngleMultiplier { get; set; } = new Angles( 1, 1, 1 );
	public Vector2 AxisMultiplier { get; set; } = new Vector2( 1, 1 );

	public ViewModelSwayEffect( float multiplier = 1, float aimMultiplier = 0.2f )
	{
		m_Multiplier = multiplier;
		m_AimMultiplier = aimMultiplier;
	}

	private Rotation m_LastSwayRot = Rotation.Identity;
	private Vector3 m_LastSwayPos;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var rot = setup.Rotation;

		var mouse = Mouse.Delta;
		mouse *= MathX.Lerp( m_Multiplier, m_AimMultiplier, setup.Hands.Aim );

		var targetRot = Rotation.From( mouse.y * AngleMultiplier.pitch, (-mouse.x * 2) * AngleMultiplier.yaw, mouse.x * AngleMultiplier.roll );
		m_LastSwayRot = Rotation.Slerp( m_LastSwayRot, targetRot, Damping * Time.Delta );

		var targetPos = rot * new Vector3( 0, (mouse.x / 2) * AxisMultiplier.y, (mouse.y / 2) * AxisMultiplier.x );
		m_LastSwayPos = m_LastSwayPos.LerpTo( targetPos, Damping * Time.Delta );

		setup.Hands.Angles *= m_LastSwayRot;
		setup.Hands.Offset += m_LastSwayPos;
	}
}
