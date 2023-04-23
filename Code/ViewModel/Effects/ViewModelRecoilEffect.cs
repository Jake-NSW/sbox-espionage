using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelRecoilEffect : IViewModelEffect
{
	public float RecoilSnap { get; init; }
	public float RecoilReturnSpeed { get; init; }
	public float RecoilViewAnglesMultiplier { get; init; }
	public float RecoilRotationMultiplier { get; init; }
	public float RecoilCameraRotationMultiplier { get; init; }

	public float KickbackSnap { get; init; }
	public float KickbackReturnSpeed { get; init; }

	public ViewModelRecoilEffect() { }

	public bool Update( ref ViewModelSetup setup )
	{
		RecoilUpdate( ref setup );
		KickbackUpdate( ref setup );

		// Remove when we are done with the effect
		return false;
	}

	private Rotation m_RecoilCurrentRotation;
	private Vector3 m_RecoilTargetRotation;

	private void RecoilUpdate( ref ViewModelSetup setup )
	{
		var rot = setup.Initial.Rotation;

		m_RecoilTargetRotation = m_RecoilTargetRotation.LerpTo( Vector3.Zero, RecoilReturnSpeed * Time.Delta );
		m_RecoilCurrentRotation = Rotation.Slerp( m_RecoilCurrentRotation, Rotation.From( m_RecoilTargetRotation.x, m_RecoilTargetRotation.y, m_RecoilTargetRotation.z ), RecoilSnap * Time.Delta );

		setup.Rotation *= m_RecoilCurrentRotation * 1.5f * RecoilRotationMultiplier;
		setup.Position += (rot.Forward * (m_RecoilCurrentRotation.Pitch()) / 2) + (rot.Left * m_RecoilCurrentRotation.Yaw() / 2);

		// add this back when I support it...
		// camSetup.Rotation *= Rotation.From( m_RecoilCurrentRotation.Angles() / 1.5f ) * RecoilCameraRotationMultiplier;
	}

	private Vector3 m_KickbackCurrentPosition;
	private Vector3 m_KickbackTargetPosition;

	public void KickbackUpdate( ref ViewModelSetup setup )
	{
		var rot = setup.Initial.Rotation;

		m_KickbackTargetPosition = m_KickbackTargetPosition.LerpTo( Vector3.Zero, KickbackReturnSpeed * Time.Delta );
		m_KickbackCurrentPosition = m_KickbackCurrentPosition.LerpTo( m_KickbackTargetPosition, KickbackSnap * Time.Delta );

		setup.Position += (rot.Forward * m_KickbackCurrentPosition.x) + (rot.Left * m_KickbackCurrentPosition.y) + (rot.Down * m_KickbackCurrentPosition.z);
	}

	public void Register( IDispatchRegistryTable table )
	{
		// OnShoot( Vector3 recoilIntensity, Vector3 kickbackIntensity )
		// m_RecoilTargetRotation += new Vector3( recoilIntensity.x, Rand.Float( -recoilIntensity.y, recoilIntensity.y ), Game.Random.Float( -recoilIntensity.z, recoilIntensity.z ) ) * Time.Delta;
		// m_KickbackTargetPosition += new Vector3( kickbackIntensity.x, Rand.Float( -kickbackIntensity.y, kickbackIntensity.y ), Game.Random.Float( -kickbackIntensity.z, kickbackIntensity.z ) ) * Time.Delta;
	}

	public void Unregister( IDispatchRegistryTable table ) { }
}
