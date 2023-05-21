using Woosh.Common;

namespace Woosh.Espionage;

public sealed class ViewModelOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public Vector3 Hip { get; set; }
	public Vector3 Aim { get; set; }

	public ViewModelOffsetEffect( Vector3 hip, Vector3 aim )
	{
		Hip = hip;
		Aim = aim;
	}

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var target = Vector3.Lerp( Hip, Aim, setup.Hands.Aim );
		var rot = setup.Rotation;
		setup.Hands.Offset += target.x * rot.Forward + target.y * rot.Left + target.z * rot.Up;
	}
}
