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

	public bool Update( ref ViewModelSetup setup )
	{
		var target = Vector3.Lerp( m_Hip, m_Aim, setup.Aim );
		var rot = setup.Initial.Rotation;
		setup.Position += target.x * rot.Forward + target.y * rot.Left + target.z * rot.Up;

		return false;
	}

	void IViewModelEffect.Register( IDispatchRegistryTable table ) { }
	void IViewModelEffect.Unregister( IDispatchRegistryTable table ) { }
}
