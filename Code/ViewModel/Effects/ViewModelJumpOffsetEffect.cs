using Sandbox;
using Sandbox.Utility;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelJumpOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float RotMulti { get; set; } = 2;
	public float PosMulti { get; set; } = 1;

	protected override void OnAutoRegister()
	{
		base.OnAutoRegister();
		// When we have a OnLandedEvent, we should play a bounce animation here.
	}

	protected override void OnActivate()
	{
		base.OnActivate();

		(Entity.Owner as IObservableEntity)?.Events.Register<PawnLanded>( OnLanded );
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		(Entity.Owner as IObservableEntity)?.Events.Unregister<PawnLanded>( OnLanded );
	}

	private void OnLanded( Event<PawnLanded> evt )
	{
		m_SinceLanded = 0;
		Log.Info("Landed");
	}

	private TimeSince m_SinceLanded;

	private float m_Offset;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		m_Offset = m_Offset.LerpTo( Game.LocalPawn.Velocity.z / 75, 15 * Time.Delta );
		m_Offset = m_Offset.Clamp( -8, 8 );

		setup.Hands.Offset += setup.Rotation.Up * (m_Offset / 2f) * PosMulti;
		setup.Hands.Angles *= Rotation.From( m_Offset * RotMulti, 0, 0 );

		var normal = (m_SinceLanded / 0.5f).Min( 1 );
		var eased = Easing.QuadraticInOut( normal );

		setup.Rotation *= Rotation.FromPitch( eased * (1 - eased) * 90);
	}
}
