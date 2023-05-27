using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

// Copied from https://github.com/Facepunch/sandbox/blob/master/code/ViewModel.cs

public sealed class SandboxViewModelEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	private float SwingInfluence => 0.05f;
	private float ReturnSpeed => 5.0f;
	private float MaxOffsetLength => 10.0f;
	private float BobCycleTime => 7;
	private Vector3 BobDirection => new Vector3( 0.0f, 1.0f, 0.5f );

	private Vector3 swingOffset;
	private float lastPitch;
	private float lastYaw;
	private float bobAnim;

	public bool EnableSwingAndBob { get; set; } = true;

	public float YawInertia { get; private set; }
	public float PitchInertia { get; private set; }

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var cameraBoneIndex = Entity.GetBoneIndex( "camera" );
		if ( cameraBoneIndex != -1 )
		{
			setup.Hands.Angles *= (setup.Rotation.Inverse * Entity.GetBoneTransform( cameraBoneIndex ).Rotation);
		}

		var newPitch = setup.Hands.Angles.Pitch();
		var newYaw = setup.Hands.Angles.Yaw();

		PitchInertia = Angles.NormalizeAngle( newPitch - lastPitch );
		YawInertia = Angles.NormalizeAngle( lastYaw - newYaw );

		if ( EnableSwingAndBob )
		{
			var playerVelocity = Game.LocalPawn.Velocity;

			var verticalDelta = playerVelocity.z * Time.Delta;
			var viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
			verticalDelta *= (1.0f - System.MathF.Abs( viewDown.Cross( Vector3.Down ).y ));
			var pitchDelta = PitchInertia - verticalDelta * 1;
			var yawDelta = YawInertia;

			var offset = CalcSwingOffset( pitchDelta, yawDelta );
			offset += CalcBobbingOffset( playerVelocity );

			setup.Hands.Offset += setup.Rotation * setup.Hands.Angles * offset;
		}
		else
		{
			Entity.SetAnimParameter( "aim_yaw_inertia", YawInertia );
			Entity.SetAnimParameter( "aim_pitch_inertia", PitchInertia );
		}

		lastPitch = newPitch;
		lastYaw = newYaw;
	}

	private Vector3 CalcSwingOffset( float pitchDelta, float yawDelta )
	{
		Vector3 swingVelocity = new Vector3( 0, yawDelta, pitchDelta );

		swingOffset -= swingOffset * ReturnSpeed * Time.Delta;
		swingOffset += (swingVelocity * SwingInfluence);

		if ( swingOffset.Length > MaxOffsetLength )
		{
			swingOffset = swingOffset.Normal * MaxOffsetLength;
		}

		return swingOffset;
	}

	private Vector3 CalcBobbingOffset( Vector3 velocity )
	{
		bobAnim += Time.Delta * BobCycleTime;

		var twoPI = System.MathF.PI * 2.0f;

		if ( bobAnim > twoPI )
		{
			bobAnim -= twoPI;
		}

		var speed = new Vector2( velocity.x, velocity.y ).Length;
		speed = speed > 10.0 ? speed : 0.0f;
		var offset = BobDirection * (speed * 0.005f) * System.MathF.Cos( bobAnim );
		offset = offset.WithZ( -System.MathF.Abs( offset.z ) );

		return offset;
	}
}
