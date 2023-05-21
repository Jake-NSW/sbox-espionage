using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct EspEffectStack : ITemplate<CompositedViewModel>
{
	public void Generate( CompositedViewModel view )
	{
		view.Components.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
		view.Components.Add( new ViewModelSwayEffect( 1, 1.3f ) );
		view.Components.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
		view.Components.Add( new ViewModelStrafeOffsetEffect() { Damping = 6, RollMultiplier = 1, AxisMultiplier = 10 } );
		view.Components.Add( new ViewModelDeadzoneSwayEffect( new Vector2( 10, 10 ) ) { AimingOnly = true, AutoCenter = false, Damping = 8 } );
		view.Components.Add( new ViewModelPitchOffsetEffect( 5, 4 ) { Damping = 15 } );
		view.Components.Add( new ViewModelRecoilEffect() );
		view.Components.Add( new ViewModelTuckEffect() );
	}
}
