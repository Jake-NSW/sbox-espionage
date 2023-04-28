using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelStrafeOffsetEffect : IViewModelEffect
{
	public float Damping { get; init; } = 6;
	public float AxisMultiplier { get; init; } = 10;
	public float RollMultiplier { get; init; } = 1;

	public float AngleClamp { get; init; } = 20;

	public ViewModelStrafeOffsetEffect() { }

	private float m_LastStrafeOffset;

	public bool Update( ref ViewModelSetup setup )
	{
		var rot = setup.Initial.Rotation;
		m_LastStrafeOffset = m_LastStrafeOffset.LerpTo( -setup.Initial.NormalToLocal( setup.Velocity ).y / 180, Damping * Time.Delta );
		m_LastStrafeOffset = m_LastStrafeOffset.Clamp( -10, 10 );

		setup.Rotation *= Rotation.From( 0, 0, m_LastStrafeOffset * AxisMultiplier );
		setup.Rotation = setup.Rotation.Clamp( setup.Initial.Rotation, AngleClamp);
		
		// setup.Position += rot.Left * (m_LastStrafeOffset / 4 * RollMultiplier);
		
		return false;
	}

	public void Register( IDispatchRegistryTable table ) { }
	public void Unregister( IDispatchRegistryTable table ) { }
}
