using Sandbox;

namespace Woosh.Espionage;

public sealed class ThirdPersonEntityCamera : EntityCameraController<Pawn>
{
	protected override void OnUpdate( ref CameraSetup setup, in InputContext input )
	{
		var center = Entity.Position + Vector3.Up * 64;
		var rot = input.ViewAngles.ToRotation() * Rotation.FromAxis( Vector3.Up, -16 );

		var distance = 130.0f * Entity.Scale;
		var targetPos = center + rot.Right * ((Entity.CollisionBounds.Mins.x + 32) * Entity.Scale);
		targetPos += rot.Forward * -distance;

		var tr = Trace.Ray( center, targetPos )
			.WithAnyTags( "solid" )
			.Ignore( Entity )
			.Radius( 8 )
			.Run();

		setup.Position = tr.EndPosition;
		setup.Rotation = rot;
	}
}
