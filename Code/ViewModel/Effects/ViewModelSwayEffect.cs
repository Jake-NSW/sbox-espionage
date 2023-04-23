using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelSwayEffect : IViewModelEffect
{
	private readonly float m_Multiplier;
	private readonly float m_AimMultiplier;

	public ViewModelSwayEffect( float multiplier = 1, float aimMultiplier = 0.2f )
	{
		m_Multiplier = multiplier;
		m_AimMultiplier = aimMultiplier;
	}

	private Vector2 m_LastDelta;
	private Rotation m_LastSwayRot;
	private Vector3 m_LastSwayPos;

	public bool Update( ref ViewModelSetup setup )
	{
		var rot = setup.Initial.Rotation;
		
		m_LastDelta = Vector2.Lerp( m_LastDelta, (Mouse.Delta / Time.Delta).Clamp( -10, 10 ) / 3.5f, 45 * Time.Delta );

		var mouse = m_LastDelta;
		mouse *= MathX.Lerp( m_Multiplier, m_AimMultiplier, setup.Aim );

		m_LastSwayRot = Rotation.Slerp( m_LastSwayRot, Rotation.From( mouse.y.Clamp( -5, 5 ), -mouse.x * 2, mouse.x ), 6 * RealTime.Delta );
		m_LastSwayPos = m_LastSwayPos.LerpTo( rot.Up * mouse.y.Clamp( -5, 5 ) / 2 + rot.Left * mouse.x / 2, 6 * RealTime.Delta );

		setup.Rotation *= m_LastSwayRot;
		setup.Position += m_LastSwayPos;

		return false;
	}

	public void Register( IDispatchRegistryTable table ) { }

	public void Unregister( IDispatchRegistryTable table ) { }
}
