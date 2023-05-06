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

	private Rotation m_LastOffsetRot = Rotation.Identity;
	private Vector3 m_LastOffsetPos;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var offset = Camera.Rotation.Pitch().Remap( -90, 90, -1, 1 );
		var rot = setup.Rotation;

		m_LastOffsetRot = Rotation.Slerp( m_LastOffsetRot, Rotation.Lerp( Rotation.From( offset * m_PitchOffset, 0, 0 ), Rotation.Identity, setup.Hands.Aim ), Damping * Time.Delta );
		m_LastOffsetPos = m_LastOffsetPos.LerpTo( Vector3.Lerp( rot.Up * offset * m_YawOffset, Vector3.Zero, setup.Hands.Aim ), Damping * Time.Delta );

		setup.Hands.Angles *= m_LastOffsetRot;
		setup.Hands.Offset += m_LastOffsetPos;
	}
}
