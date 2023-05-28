using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelRampOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	private float m_LastHeight;
	private float m_DampedHeight;

	protected override void OnActivate()
	{
		base.OnActivate();

		m_LastHeight = 0;
		m_DampedHeight = m_LastHeight;
	}

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		m_DampedHeight = m_DampedHeight.LerpTo( Entity.Owner.GroundEntity == null ? 0 : m_LastHeight - Entity.Owner.Position.z, 3 * Time.Delta );
		m_LastHeight = Entity.Owner.Position.z;

		var alpha = 1 - setup.Hands.Aim;
		setup.Hands.Offset += setup.Rotation * new Vector3( 0, 0, m_DampedHeight * 6.5f * alpha );
		setup.Hands.Angles *= Rotation.FromPitch( m_DampedHeight * 15 * alpha );
	}
}
