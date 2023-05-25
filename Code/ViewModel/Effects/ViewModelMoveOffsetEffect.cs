using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelMoveOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
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

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var rot = setup.Rotation;
		var velocity = Entity.Owner.Velocity;

		m_LastMoveOffset = m_LastMoveOffset.LerpTo( setup.Transform.NormalToLocal( velocity ).x / 90, m_Damping * Time.Delta );
		m_LastMoveOffset = m_LastMoveOffset.Clamp( -5, 5 );

		m_LastSidewayMoveOffset = m_LastSidewayMoveOffset.LerpTo( -setup.Transform.NormalToLocal( velocity ).y / 180, m_Damping * Time.Delta );
		m_LastSidewayMoveOffset = m_LastSidewayMoveOffset.Clamp( -5, 5 );

		setup.Hands.Offset += rot.Backward * (m_LastMoveOffset / 1.5f * m_Multiplier.x);
		setup.Hands.Offset += rot.Down * (m_LastMoveOffset / 4 * m_Multiplier.z);
		setup.Hands.Offset += rot.Left * (m_LastSidewayMoveOffset / 1.5f * m_Multiplier.y);
	}
}
