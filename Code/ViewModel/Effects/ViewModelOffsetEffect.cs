using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public Vector3 Hip { get; set; }
	public Vector3 Aim { get; set; }
	public Vector3 Angles { get; set; }

	public ViewModelOffsetEffect( Vector3 hip, Vector3 aim, Vector3 angles = default )
	{
		Hip = hip;
		Aim = aim;
		Angles = angles;
	}

	public void OnPostSetup( ref CameraSetup setup )
	{
		var target = Vector3.Lerp( Hip, Aim, setup.Hands.Aim );
		var rot = setup.Rotation.WithRoll( 0 );
		setup.Hands.Offset += rot * target;

		if ( Angles != Vector3.Zero )
		{
			setup.Hands.Angles *= Rotation.From( Angles.x, Angles.y, Angles.z );
		}
	}
}
