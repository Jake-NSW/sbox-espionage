using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelDeadzoneSwayEffect : IViewModelEffect
{
	private readonly Vector2 m_Deadzone;

	public bool AimingOnly { get; init; } = false;
	public bool AutoCenter { get; init; } = true;

	public float Damping { get; init; } = 8;
	public float Multiplier { get; init; } = 1;

	public ViewModelDeadzoneSwayEffect( Vector2 deadzone )
	{
		m_Deadzone = deadzone;
	}

	private Vector2 m_SavedDeadzoneAxis;
	private Rotation m_LastDeadzoneRotation = Rotation.Identity;

	public bool Update( ref ViewModelSetup setup )
	{
		var isAiming = setup.Aim > 0.1f;
		DeadzoneAxis( ref setup, m_Deadzone );

		if ( AutoCenter || (AimingOnly && !isAiming) )
			m_SavedDeadzoneAxis.x = m_SavedDeadzoneAxis.x.LerpTo( 0, 2f * Time.Delta );

		if ( AutoCenter || (AimingOnly && !isAiming) )
			m_SavedDeadzoneAxis.y = m_SavedDeadzoneAxis.y.LerpTo( 0, 2f * Time.Delta );

		var axis = Rotation.From( m_SavedDeadzoneAxis.x, m_SavedDeadzoneAxis.y, 0 );
		m_LastDeadzoneRotation = Rotation.Slerp( m_LastDeadzoneRotation, AimingOnly ? Rotation.Lerp( Rotation.Identity, axis, setup.Aim ) : axis, Damping * Time.Delta );

		setup.Rotation *= m_LastDeadzoneRotation;

		var eular = m_LastDeadzoneRotation.Angles();
		// setup.Position += rot.Up * (eular.pitch / 20) + rot.Right * (eular.yaw / 20);
		// setup.Position += rot.Up *  m_LastDeadzoneRotation.x + rot.Right * m_LastDeadzoneRotation.y;

		return false;
	}

	private void DeadzoneAxis( ref ViewModelSetup setup, Vector2 deadZoneBox )
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

	public void Register( IDispatchRegistryTable table )
	{
		table.Register(
			( in WeaponFireEvent evt ) =>
			{
				var recoil = evt.Recoil.Abs() / 4;
				var x = Game.Random.Float( -recoil.x, recoil.x ) / 24;

				m_SavedDeadzoneAxis.x -= recoil.y;
				m_SavedDeadzoneAxis.y -= x;
			}
		);
	}

	public void Unregister( IDispatchRegistryTable table ) { }
}
