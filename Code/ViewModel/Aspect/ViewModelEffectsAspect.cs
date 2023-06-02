using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct ViewModelEffectsAspect : IAspect<CompositedViewModel>
{
	public string Model { get; }
	public TuckType HipTuck { get; init; } = TuckType.Push;
	public TuckType AimTuck { get; init; } = TuckType.Push;

	public ViewModelEffectsAspect( string model )
	{
		Model = model;
	}

	public void Fill( CompositedViewModel view )
	{
		if ( Model != null )
			view.Model = Sandbox.Model.Load( Model );

		view.Components.Add( new ViewModelCameraAnimationEffect() );

		view.Components.Add( new ViewModelSwayEffect( 1, 0.4f ) );

		view.Components.Add( new ViewModelRampOffsetEffect() );
		view.Components.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
		view.Components.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
		view.Components.Add( new ViewModelJumpOffsetEffect() );

		view.Components.Add( new ViewModelMoveBobEffect() );
		view.Components.Add( new ViewModelBreatheEffect() );

		view.Components.Add(
			new ViewModelStrafeOffsetEffect()
			{
				Damping = 6,
				Roll = 1,
				Axis = 10
			}
		);
		view.Components.Add( new ViewModelPitchOffsetEffect( 5, 4 ) { Damping = 15 } );

		view.Components.Add( new ViewModelRecoilEffect() );
		view.Components.Add( new ViewModelKickbackEffect() );
		
		view.Components.Add( new ViewModelTuckEffect() { HipVariant = HipTuck, AimVariant = AimTuck } );
		view.Components.Create<ViewModelProjectilePredictor>();
	}
}
