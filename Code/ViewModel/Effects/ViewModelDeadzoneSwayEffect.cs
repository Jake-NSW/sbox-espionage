using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class ViewModelDeadzoneSwayEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	private readonly Vector2 m_Deadzone;

	public bool AimingOnly { get; set; } = false;
	public bool AutoCenter { get; set; } = true;

	public float Damping { get; set; } = 8;
	public float Multiplier { get; set; } = 1;

	public ViewModelDeadzoneSwayEffect( Vector2 deadzone )
	{
		m_Deadzone = deadzone;
	}

	private Vector2 m_SavedDeadzoneAxis;
	private Rotation m_LastDeadzoneRotation = Rotation.Identity;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var isAiming = setup.Hands.Aim > 0.1f;
		DeadzoneAxis( in setup.Hands, m_Deadzone );

		if ( AutoCenter || (AimingOnly && !isAiming) )
			m_SavedDeadzoneAxis.x = m_SavedDeadzoneAxis.x.LerpTo( 0, 2f * Time.Delta );

		if ( AutoCenter || (AimingOnly && !isAiming) )
			m_SavedDeadzoneAxis.y = m_SavedDeadzoneAxis.y.LerpTo( 0, 2f * Time.Delta );

		var axis = Rotation.From( m_SavedDeadzoneAxis.x, m_SavedDeadzoneAxis.y, 0 );
		m_LastDeadzoneRotation = Rotation.Slerp( m_LastDeadzoneRotation, AimingOnly ? Rotation.Lerp( Rotation.Identity, axis, setup.Hands.Aim ) : axis, Damping * Time.Delta );
		setup.Hands.Angles *= m_LastDeadzoneRotation;
	}

	private void DeadzoneAxis( in ViewModelSetup setup, Vector2 deadZoneBox )
	{
		m_SavedDeadzoneAxis.x += Mouse.Delta.y * 20 * Multiplier * Time.Delta;
		m_SavedDeadzoneAxis.x = m_SavedDeadzoneAxis.x.Clamp( -deadZoneBox.x, deadZoneBox.x );

		m_SavedDeadzoneAxis.y += -Mouse.Delta.x * 20 * Multiplier * Time.Delta;
		m_SavedDeadzoneAxis.y = m_SavedDeadzoneAxis.y.Clamp( -deadZoneBox.y, deadZoneBox.y );

		if ( AimingOnly )
		{
			m_SavedDeadzoneAxis *= setup.Aim;
		}
	}
}
