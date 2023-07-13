using System;
using Sandbox;
using Sandbox.Utility;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelJumpOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float RotMulti { get; set; } = 2;
	public float PosMulti { get; set; } = 1;

	[Listen]
	private void OnLanded( Event<PawnLanded> evt )
	{
		m_SinceLanded = 0;
		m_Random = Vector3.Random;
		m_LandVelocity = evt.Data.Velocity;
	}

	private TimeSince m_SinceLanded;
	private Vector3 m_Random;
	private Vector3 m_LandVelocity;

	private float m_Offset;

	public void OnPostMutate( ref CameraSetup setup )
	{
		var rot = setup.Rotation.WithRoll(0);
		
		m_Offset = m_Offset.LerpTo( Game.LocalPawn.Velocity.z / 75, 15 * Time.Delta );
		m_Offset = m_Offset.Clamp( -8, 8 );

		setup.Hands.Offset += rot.Up * (m_Offset / 2f) * PosMulti;
		setup.Hands.Angles *= Rotation.From( m_Offset * RotMulti, 0, 0 );

		// Land Effects

		var normal = (m_SinceLanded / 1f).Min( 1 );
		var eased =Easing.ExpoOut( normal );

		var random = (2 * (MathF.Sin( m_Random.LengthSquared ).Min( 1 ))) - 1;
		var curved = eased * (1 - eased);
		var offset = new Vector3( 0, -m_LandVelocity.y / 50, m_LandVelocity.z / 80 );
		
		setup.Hands.Offset += offset * curved;
		setup.Hands.Angles *= Rotation.FromPitch( curved * -offset.z );
		setup.Hands.Angles *= Rotation.FromRoll( curved * random * -m_LandVelocity.z / 50 );
	}
}
