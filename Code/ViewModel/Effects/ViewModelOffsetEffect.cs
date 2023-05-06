namespace Woosh.Espionage;

public sealed class ViewModelOffsetEffect : IViewModelEffect
{
	private readonly Vector3 m_Hip;
	private readonly Vector3 m_Aim;

	public ViewModelOffsetEffect( Vector3 hip, Vector3 aim )
	{
		m_Hip = hip;
		m_Aim = aim;
	}

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var target = Vector3.Lerp( m_Hip, m_Aim, setup.Hands.Aim );
		var rot = setup.Rotation;
		setup.Hands.Offset += target.x * rot.Forward + target.y * rot.Left + target.z * rot.Up;
	}
}
