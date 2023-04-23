using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelSwayEffect : IViewModelEffect
{
	private readonly float m_Multiplier;
	private readonly float m_AimMultiplier;

	public float Damping { get; init; } = 6;
	public Angles AngleMultiplier = new Angles( 1, 1, 1 );

	public ViewModelSwayEffect( float multiplier = 1, float aimMultiplier = 0.2f )
	{
		m_Multiplier = multiplier;
		m_AimMultiplier = aimMultiplier;
	}

	private Rotation m_LastSwayRot;
	private Vector3 m_LastSwayPos;

	public bool Update( ref ViewModelSetup setup )
	{
		var rot = setup.Initial.Rotation;

		var mouse = Mouse.Delta;
		mouse *= MathX.Lerp( m_Multiplier, m_AimMultiplier, setup.Aim );

		var targetRot = Rotation.From( mouse.y.Clamp( -5, 5 ) * AngleMultiplier.pitch, (-mouse.x * 2) * AngleMultiplier.yaw, mouse.x * AngleMultiplier.roll );
		m_LastSwayRot = Rotation.Slerp( m_LastSwayRot, targetRot, Damping * RealTime.Delta );
		m_LastSwayPos = m_LastSwayPos.LerpTo( rot.Up * mouse.y.Clamp( -5, 5 ) / 2 + rot.Left * mouse.x / 2, Damping * RealTime.Delta );

		setup.Rotation *= m_LastSwayRot;
		setup.Position += m_LastSwayPos;

		return false;
	}

	public void Register( IDispatchRegistryTable table ) { }
	public void Unregister( IDispatchRegistryTable table ) { }
}
