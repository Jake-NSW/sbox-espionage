using System;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class ViewModelRecoilEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float RecoilSnap { get; init; } = 25;
	public float RecoilReturnSpeed { get; init; } = 5;
	public float RecoilViewAnglesMultiplier { get; init; } = 6f;
	public float RecoilRotationMultiplier { get; init; } = 1;
	public float RecoilCameraRotationMultiplier { get; init; } = 1;

	public float KickbackSnap { get; init; } = 25;
	public float KickbackReturnSpeed { get; init; } = 12;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		ApplyRecoil( ref setup );
		ApplyKickback( ref setup );
	}

	private Rotation m_RecoilCurrentRotation = Rotation.Identity;
	private Vector3 m_RecoilTargetRotation;

	private void ApplyRecoil( ref CameraSetup setup )
	{
		var rot = setup.Rotation;

		m_RecoilTargetRotation = m_RecoilTargetRotation.LerpTo( Vector3.Zero, RecoilReturnSpeed * Time.Delta );
		m_RecoilCurrentRotation = Rotation.Slerp( m_RecoilCurrentRotation, Rotation.From( m_RecoilTargetRotation.x, m_RecoilTargetRotation.y, m_RecoilTargetRotation.z ), RecoilSnap * Time.Delta );

		setup.Hands.Angles *= m_RecoilCurrentRotation * 1.5f * RecoilRotationMultiplier;
		setup.Hands.Offset += (rot.Forward * (m_RecoilCurrentRotation.Pitch()) / 2) + (rot.Left * m_RecoilCurrentRotation.Yaw() / 2);

		// add this back when I support it...
		setup.Rotation *= Rotation.From( m_RecoilCurrentRotation.Angles() ) * RecoilCameraRotationMultiplier;
	}

	private Vector3 m_KickbackCurrentPosition;
	private Vector3 m_KickbackTargetPosition;

	public void ApplyKickback( ref CameraSetup setup )
	{
		var rot = setup.Rotation;

		m_KickbackTargetPosition = m_KickbackTargetPosition.LerpTo( Vector3.Zero, KickbackReturnSpeed * Time.Delta );
		m_KickbackCurrentPosition = m_KickbackCurrentPosition.LerpTo( m_KickbackTargetPosition, KickbackSnap * Time.Delta );

		setup.Hands.Offset += (rot.Forward * m_KickbackCurrentPosition.x) + (rot.Left * m_KickbackCurrentPosition.y) + (rot.Down * m_KickbackCurrentPosition.z);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Register<WeaponFired>( OnShoot );
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		Unregister<WeaponFired>( OnShoot );
	}

	private void OnShoot( Event<WeaponFired> evt )
	{
		var rand = Game.Random;
		var recoil = evt.Data.Recoil;
		var kickback = evt.Data.Kickback;

		m_RecoilTargetRotation += new Vector3( recoil.x, rand.Float( -recoil.y, recoil.y ), Game.Random.Float( -recoil.z, recoil.z ) ) * Time.Delta;
		m_KickbackTargetPosition += new Vector3( kickback.x, rand.Float( -kickback.y, kickback.y ), Game.Random.Float( -kickback.z, kickback.z ) ) * Time.Delta;
	}
}
