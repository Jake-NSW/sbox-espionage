using Woosh.Common;

namespace Woosh.Espionage;

public sealed class ViewModelCameraAnimationEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var cameraAnimTarget = Entity.GetAttachment( "camera_anim_target", false );
		if ( cameraAnimTarget.HasValue )
		{
			setup.Rotation *= (cameraAnimTarget.Value.Rotation * Rotation.From( -90, -90, 0 ));
		}
	}
}
