using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelCameraAnimationEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float Scale { get; set; } = 1;
	
	public void OnPostMutate( ref CameraSetup setup )
	{
		var target = Entity.GetAttachment( "camera_anim_target", false );
		if ( !target.HasValue )
		{
			return;
		}

		setup.Rotation *= Rotation.Lerp( Rotation.Identity, target.Value.Rotation * Rotation.From( -90, -90, 0 ), Scale, false );
	}
}
