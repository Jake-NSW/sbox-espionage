using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelJumpOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float RotMulti { get; set; } = 1;
	public float PosMulti { get; set; } = 1;

	private float m_Offset;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		m_Offset = m_Offset.LerpTo( Game.LocalPawn.Velocity.z / 75, 15 * Time.Delta );
		m_Offset = m_Offset.Clamp( -8, 8 );

		setup.Hands.Offset += setup.Rotation.Up * m_Offset * RotMulti;
		setup.Hands.Angles *= Rotation.From( m_Offset * PosMulti, 0, 0 );
	}
}
