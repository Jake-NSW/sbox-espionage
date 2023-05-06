namespace Woosh.Espionage;

public readonly struct EspEffectStack : ITemplate<CompositedViewModel>
{
	public void Generate( CompositedViewModel view )
	{
		view.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
		view.Add( new ViewModelSwayEffect( 1, 1.3f ) );
		view.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
		view.Add( new ViewModelStrafeOffsetEffect() { Damping = 6, RollMultiplier = 1, AxisMultiplier = 10 } );
		view.Add( new ViewModelDeadzoneSwayEffect( new Vector2( 10, 10 ) ) { AimingOnly = true, AutoCenter = false, Damping = 8 } );
		view.Add( new ViewModelPitchOffsetEffect( 5, 4 ) { Damping = 15 } );
		view.Add( new ViewModelRecoilEffect() );
		view.Add( new ViewModelTuckEffect() );
	}
}
