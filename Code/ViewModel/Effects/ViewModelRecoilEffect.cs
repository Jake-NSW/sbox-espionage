using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelRecoilEffect : IViewModelEffect
{
	public float RecoilSnap { get; init; } = 25;
	public float RecoilReturnSpeed { get; init; } = 20;
	public float RecoilViewAnglesMultiplier { get; init; } = 6f;
	public float RecoilRotationMultiplier { get; init; } = 1;
	public float RecoilCameraRotationMultiplier { get; init; } = 1;

	public float KickbackSnap { get; init; } = 25;
	public float KickbackReturnSpeed { get; init; } = 12;

	public ViewModelRecoilEffect() { }

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		RecoilUpdate( ref setup );
		KickbackUpdate( ref setup );
	}

	private Rotation m_RecoilCurrentRotation = Rotation.Identity;
	private Vector3 m_RecoilTargetRotation;

	private void RecoilUpdate( ref CameraSetup setup )
	{
		var rot = setup.Rotation;

		m_RecoilTargetRotation = m_RecoilTargetRotation.LerpTo( Vector3.Zero, RecoilReturnSpeed * Time.Delta );
		m_RecoilCurrentRotation = Rotation.Slerp( m_RecoilCurrentRotation, Rotation.From( m_RecoilTargetRotation.x, m_RecoilTargetRotation.y, m_RecoilTargetRotation.z ), RecoilSnap * Time.Delta );

		setup.Hands.Angles *= m_RecoilCurrentRotation * 1.5f * RecoilRotationMultiplier;
		setup.Hands.Offset += (rot.Forward * (m_RecoilCurrentRotation.Pitch()) / 2) + (rot.Left * m_RecoilCurrentRotation.Yaw() / 2);

		// add this back when I support it...
		setup.Rotation *= Rotation.From( m_RecoilCurrentRotation.Angles() / 1.5f ) * RecoilCameraRotationMultiplier;
	}

	private Vector3 m_KickbackCurrentPosition;
	private Vector3 m_KickbackTargetPosition;

	public void KickbackUpdate( ref CameraSetup setup )
	{
		var rot = setup.Rotation;

		m_KickbackTargetPosition = m_KickbackTargetPosition.LerpTo( Vector3.Zero, KickbackReturnSpeed * Time.Delta );
		m_KickbackCurrentPosition = m_KickbackCurrentPosition.LerpTo( m_KickbackTargetPosition, KickbackSnap * Time.Delta );

		setup.Hands.Offset += (rot.Forward * m_KickbackCurrentPosition.x) + (rot.Left * m_KickbackCurrentPosition.y) + (rot.Down * m_KickbackCurrentPosition.z);
	}

	public void Register( AnimatedEntity entity, IDispatchRegistryTable table )
	{
		table?.Register(
			( in WeaponFireEvent evt ) =>
			{
				var rand = Game.Random;
				m_RecoilTargetRotation += new Vector3( evt.Recoil.x, rand.Float( -evt.Recoil.y, evt.Recoil.y ), Game.Random.Float( -evt.Recoil.z, evt.Recoil.z ) ) * Time.Delta;
				m_KickbackTargetPosition += new Vector3( evt.Kickback.x, rand.Float( -evt.Kickback.y, evt.Kickback.y ), Game.Random.Float( -evt.Kickback.z, evt.Kickback.z ) ) * Time.Delta;
			}
		);

		// OnShoot( Vector3 recoilIntensity, Vector3 kickbackIntensity )
	}

	public void Unregister( IDispatchRegistryTable table ) { }
}
