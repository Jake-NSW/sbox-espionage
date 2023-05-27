using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelJumpOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float RotMulti { get; set; } = 2;
	public float PosMulti { get; set; } = 1;

	protected override void OnAutoRegister()
	{
		base.OnAutoRegister();
		// When we have a OnLandedEvent, we should play a bounce animation here.
	}

	private float m_Offset;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		m_Offset = m_Offset.LerpTo( Game.LocalPawn.Velocity.z / 75, 15 * Time.Delta );
		m_Offset = m_Offset.Clamp( -8, 8 );

		setup.Hands.Offset += setup.Rotation.Up * (m_Offset / 2f) * PosMulti;
		setup.Hands.Angles *= Rotation.From( m_Offset * RotMulti, 0, 0 );
	}
}
