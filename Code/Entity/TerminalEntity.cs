using Editor;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

[Library( "esp_terminal" ), Icon( "terminal" ), Category( "Gameplay" )]
[HammerEntity, Model]
public sealed partial class TerminalEntity : AnimatedEntity, IControllable, IHave<DisplayInfo>, IMutateCameraSetup
{
	[Net] [Property] public string Display { get; set; }

	DisplayInfo IHave<DisplayInfo>.Item => new DisplayInfo()
	{
		Name = Display, Icon = "terminal",
	};

	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
	}

	public bool IsUsable( Entity pawn )
	{
		return true;
	}

	public void Entering( Entity pawn )
	{
		// Transition Camera to some point?
	}

	public void Leaving()
	{
		// Reset Camera
	}

	public bool Simulate( Entity pawn )
	{
		if ( Input.Pressed( "use" ) )
		{
			// We're Done
			return true;
		}

		return false;
	}

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		// var target = WorldSpaceBounds.Center + (Rotation.Backward * 64);
		setup.Position = setup.Position.WithZ(WorldSpaceBounds.Center.z);
		setup.Rotation = Rotation.LookAt( WorldSpaceBounds.Center - setup.Position );
		setup.Viewer = null;
	}
}
