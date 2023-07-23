using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelStrafeOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float Damping { get; set; } = 6;
	public float Axis { get; set; } = 10;
	public float Roll { get; set; } = 1;

	private float m_Offset;

	public void OnMutate( ref CameraSetup setup )
	{
		var rot = setup.Rotation.WithRoll( 0 );
		var velocity = Entity.Owner.Velocity;

		m_Offset = m_Offset.Damp( -setup.Transform.NormalToLocal( velocity ).y / 180, Damping , Time.Delta );
		m_Offset = m_Offset.Clamp( -10, 10 );

		setup.Hands.Angles *= Rotation.From( 0, 0, m_Offset * Axis );
		setup.Position += rot.Left * (m_Offset / 4 * Roll);
	}
}
