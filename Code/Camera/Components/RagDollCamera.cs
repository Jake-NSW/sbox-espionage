using Sandbox;

namespace Woosh.Espionage;

public sealed class RagDollCamera : ICameraController
{
	public Entity Entity { get; }

	public RagDollCamera( Entity entity )
	{
		Entity = entity;
	}

	public void Enabled( ref CameraSetup setup )
	{
		setup.Position = Entity.Position + Vector3.Up * 72 + Vector3.Backward * 96;
	}

	public void Update( ref CameraSetup setup, in InputContext input )
	{
		setup.Position = setup.Position.RotateAround( Entity.Position, Rotation.FromYaw( -input.AnalogLook.yaw * 2 ) );
		setup.Rotation = Rotation.LookAt( Entity.WorldSpaceBounds.Center - setup.Position );
	}

	public void Disabled() { }
}
