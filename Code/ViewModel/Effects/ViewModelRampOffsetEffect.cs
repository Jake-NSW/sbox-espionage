using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelRampOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	private float m_Height;
	private float m_DampedHeight;

	protected override void OnActivate()
	{
		base.OnActivate();

		m_Height = Entity.Owner.Position.z;
		m_DampedHeight = 0;
	}

	public void OnPostSetup( ref CameraSetup setup )
	{
		m_DampedHeight = m_DampedHeight.LerpTo( Entity.Owner.GroundEntity == null ? 0 : m_Height - Entity.Owner.Position.z, 3 * Time.Delta );
		m_Height = Entity.Owner.Position.z;

		var alpha = 1 - setup.Hands.Aim;
		setup.Hands.Offset += setup.Rotation.WithRoll( 0 ) * new Vector3( 0, 0, m_DampedHeight * 6.5f * alpha );
		setup.Hands.Angles *= Rotation.FromPitch( m_DampedHeight * 15 * alpha );
	}
}
