namespace Woosh.Espionage;

public sealed class FirstPersonCamera : CameraController<Pawn>
{
	public FirstPersonCamera() { }
	public FirstPersonCamera( Pawn entity ) : base( entity ) { }

	protected override void Update( ref CameraSetup setup, in InputContext input )
	{
		setup.Viewer = Entity;
		setup.Rotation = input.ViewAngles.ToRotation();
		setup.Position = Entity.EyePosition;
	}
}
