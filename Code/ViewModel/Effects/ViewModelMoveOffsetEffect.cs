using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelMoveOffsetEffect : IViewModelEffect
{
	private readonly Vector3 m_Multiplier;
	private readonly float m_Damping;

	public ViewModelMoveOffsetEffect( Vector3 multiplier, float damping = 18 )
	{
		m_Multiplier = multiplier;
		m_Damping = damping;
	}

	private float m_LastMoveOffset;
	public float m_LastSidewayMoveOffset;

	public bool Update( ref ViewModelSetup setup )
	{
		var rot = setup.Initial.Rotation;
		m_LastMoveOffset = m_LastMoveOffset.LerpTo( setup.Initial.NormalToLocal( setup.Velocity ).x / 90, m_Damping * Time.Delta );
		m_LastMoveOffset = m_LastMoveOffset.Clamp( -5, 5 );

		m_LastSidewayMoveOffset = m_LastSidewayMoveOffset.LerpTo( -setup.Initial.NormalToLocal( setup.Velocity ).y / 180, m_Damping * Time.Delta );
		m_LastSidewayMoveOffset = m_LastSidewayMoveOffset.Clamp( -5, 5 );

		setup.Position += rot.Backward * (m_LastMoveOffset / 1.5f * m_Multiplier.x);
		setup.Position += rot.Down * (m_LastMoveOffset / 4 * m_Multiplier.z);
		setup.Position += rot.Left * (m_LastSidewayMoveOffset / 1.5f * m_Multiplier.y);

		return false;
	}

	public void Register( IDispatchRegistryTable table ) { }
	public void Unregister( IDispatchRegistryTable table ) { }
}
