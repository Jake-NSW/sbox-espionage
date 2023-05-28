using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelStrafeOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float Damping { get; set; } = 6;
	public float AxisMultiplier { get; set; } = 10;
	public float RollMultiplier { get; set; } = 1;

	public float AngleClamp { get; set; } = 20;

	private float m_LastStrafeOffset;

	public void OnPostSetup( ref CameraSetup setup )
	{
		var rot = setup.Rotation;
		var velocity = Entity.Owner.Velocity;
		
		m_LastStrafeOffset = m_LastStrafeOffset.LerpTo( -setup.Transform.NormalToLocal( velocity ).y / 180, Damping * Time.Delta );
		m_LastStrafeOffset = m_LastStrafeOffset.Clamp( -10, 10 );

		setup.Hands.Angles *= Rotation.From( 0, 0, m_LastStrafeOffset * AxisMultiplier );
		// setup.Hands.Angles =  (setup.Rotation * setup.Hands.Angles).Clamp( setup.Rotation, AngleClamp);

		// setup.Position += rot.Left * (m_LastStrafeOffset / 4 * RollMultiplier);
	}
}
