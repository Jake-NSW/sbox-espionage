using Woosh.Common;

namespace Woosh.Espionage;

public sealed class ViewModelCameraAnimationEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var cameraAnimTarget = Entity.GetAttachment( "camera_anim_target" );
		if ( cameraAnimTarget.HasValue )
		{
			var target = (cameraAnimTarget.Value.Rotation * Rotation.From(-90, -90, 0)) * setup.Rotation.Inverse;
			setup.Rotation *= target;
		}
	}
}
