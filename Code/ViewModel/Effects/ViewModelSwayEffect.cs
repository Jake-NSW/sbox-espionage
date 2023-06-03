using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelSwayEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect, IMutate<InputContext>
{
	private readonly float m_Multiplier;
	private readonly float m_AimMultiplier;

	public float Damping { get; set; } = 6;

	public ViewModelSwayEffect( float multiplier = 1, float aimMultiplier = 0.2f )
	{
		m_Multiplier = multiplier;
		m_AimMultiplier = aimMultiplier;
	}

	private Angles m_LastAngles;
	private Angles m_CurrentAngles;

	private Rotation m_LastSwayRot = Rotation.Identity;
	private Vector3 m_LastSwayPos;

	void IMutate<CameraSetup>.OnPostSetup( ref CameraSetup setup )
	{
		var rot = setup.Rotation.WithRoll( 0 );

		// Workout how much we've moved since last frame
		var angles = (m_LastAngles - m_CurrentAngles).Normal;
		var mouse = new Vector2( angles.yaw, -angles.pitch ) * 4;
		mouse *= MathX.Lerp( m_Multiplier, m_AimMultiplier, setup.Hands.Aim );
		m_LastAngles = m_CurrentAngles;

		// Rotate and lerp the viewmodel
		var targetRot = Rotation.From( mouse.y, -mouse.x, mouse.x );
		m_LastSwayRot = Rotation.Lerp( m_LastSwayRot, targetRot, Damping * Time.Delta );

		// Move the viewmodel to a nice new position
		var targetPos = new Vector3( 0, mouse.x * 0.5f, mouse.y * 0.5f );
		m_LastSwayPos = m_LastSwayPos.LerpTo( targetPos, Damping * Time.Delta );

		setup.Hands.Angles *= m_LastSwayRot;
		setup.Hands.Offset += rot * m_LastSwayPos;
	}

	void IMutate<InputContext>.OnPostSetup( ref InputContext setup )
	{
		m_CurrentAngles = setup.ViewAngles;
	}
}
