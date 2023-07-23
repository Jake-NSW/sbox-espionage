using Sandbox;

namespace Woosh.Espionage;

public sealed class FirstPersonCamera : EntityComponent<Pawn>, IController<CameraSetup>
{
	public void Update( ref CameraSetup setup, in InputContext input )
	{
		setup.Viewer = Entity;
		setup.Rotation = input.ViewAngles.ToRotation();
		setup.Position = Entity.Eyes.ToWorld( Entity.Transform ).Position;
	}
}
