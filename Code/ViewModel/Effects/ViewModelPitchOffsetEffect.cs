using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelPitchOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float PitchOffset { get; set; }
	public float YawOffset { get; set; }
	public float Damping { get; set; } = 15;

	public ViewModelPitchOffsetEffect() : this( 5, 4 ) { }

	public ViewModelPitchOffsetEffect( float pitchOffset = 5, float yawOffset = 4 )
	{
		PitchOffset = pitchOffset;
		YawOffset = yawOffset;
	}

	private Rotation m_LastOffsetRot = Rotation.Identity;
	private Vector3 m_LastOffsetPos;

	public void OnPostSetup( ref CameraSetup setup )
	{
		var offset = Camera.Rotation.Pitch().Remap( -90, 90, -1, 1 );
		var rot = setup.Rotation;

		m_LastOffsetRot = m_LastOffsetRot.Damp( Rotation.Lerp( Rotation.From( offset * PitchOffset, 0, 0 ), Rotation.Identity, setup.Hands.Aim ), Damping, Time.Delta );
		m_LastOffsetPos = m_LastOffsetPos.Damp( Vector3.Lerp( rot.Up * offset * YawOffset, Vector3.Zero, setup.Hands.Aim ), Damping, Time.Delta );

		setup.Hands.Angles *= m_LastOffsetRot;
		setup.Hands.Offset += m_LastOffsetPos;
	}
}
