using System;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelRotateTowardsEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float Damping { get; set; } = 12;
	
	private readonly Func<Transform?> m_Towards;

	public ViewModelRotateTowardsEffect( Func<Transform?> towards )
	{
		m_Towards = towards ?? throw new ArgumentNullException( nameof(towards) );
	}

	private Rotation m_LookAt = Rotation.Identity;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var trans = m_Towards.Invoke();

		if ( !trans.HasValue )
		{
			m_LookAt = Rotation.Identity;
			return;
		}

		var look = Rotation.LookAt( (trans.Value.Position - setup.Position).Normal );
		var target = look * setup.Rotation.Inverse;
		
		setup.Hands.Angles = m_LookAt = Rotation.Slerp(m_LookAt, target, Damping * Time.Delta);
	}
}
