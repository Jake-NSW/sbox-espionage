using Sandbox;
using Woosh.Espionage;

namespace Woosh.Espionage;

public readonly struct ViewModelEffectsAspect : IEntityAspect<CompositedViewModel>
{
	public ViewModelEffectsAspect() { }

	public TuckType HipTuck { get; init; } = TuckType.Push;
	public TuckType AimTuck { get; init; } = TuckType.Push;


	void IEntityAspect<CompositedViewModel>.ImportFrom( CompositedViewModel value, IComponentSystem system )
	{
		// Nothing to Import
	}

	void IEntityAspect<CompositedViewModel>.ExportTo( CompositedViewModel view, IComponentSystem system )
	{
		system.Add( new ViewModelCameraAnimationEffect() );

		system.Add( new ViewModelSwayEffect( 1, 0.4f ) );

		system.Add( new ViewModelRampOffsetEffect() );
		system.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
		system.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
		system.Add( new ViewModelJumpOffsetEffect() );

		system.Add( new ViewModelMoveBobEffect() );
		system.Add( new ViewModelBreatheEffect() );

		system.Add(
			new ViewModelStrafeOffsetEffect()
			{
				Damping = 6,
				Roll = 1,
				Axis = 10
			}
		);
		system.Add( new ViewModelPitchOffsetEffect( 5, 4 ) { Damping = 15 } );

		system.Add( new ViewModelRecoilEffect() );
		system.Add( new ViewModelKickbackEffect() );
		system.Add( new ViewModelTuckEffect() { HipVariant = HipTuck, AimVariant = AimTuck } );
	}
}
