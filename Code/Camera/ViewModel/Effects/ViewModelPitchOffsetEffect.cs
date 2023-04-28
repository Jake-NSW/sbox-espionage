using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelPitchOffsetEffect : IViewModelEffect
{
	private readonly float m_PitchOffset;
	private readonly float m_YawOffset;

	public float Damping { get; init; } = 15;

	public ViewModelPitchOffsetEffect( float pitchOffset = 5, float yawOffset = 4 )
	{
		m_PitchOffset = pitchOffset;
		m_YawOffset = yawOffset;
	}

	private Rotation m_LastOffsetRot;
	private Vector3 m_LastOffsetPos;

	public bool Update( ref ViewModelSetup setup )
	{
		var offset = Camera.Rotation.Pitch().Remap( -90, 90, -1, 1 );
		var rot = setup.Initial.Rotation;

		m_LastOffsetRot = Rotation.Slerp( m_LastOffsetRot, Rotation.Lerp( Rotation.From( offset * m_PitchOffset, 0, 0 ), Rotation.Identity, setup.Aim ), Damping * Time.Delta );
		m_LastOffsetPos = m_LastOffsetPos.LerpTo( Vector3.Lerp( rot.Up * offset * m_YawOffset, Vector3.Zero, setup.Aim ), Damping * Time.Delta );

		setup.Rotation *= m_LastOffsetRot;
		setup.Position += m_LastOffsetPos;

		return false;
	}

	public void Register( IDispatchRegistryTable table ) { }
	public void Unregister( IDispatchRegistryTable table ) { }
}
