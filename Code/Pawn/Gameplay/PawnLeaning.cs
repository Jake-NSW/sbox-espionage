using Sandbox;
using Sandbox.Utility;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class PawnLeaning : ObservableEntityComponent<Pawn>, IMutate<CameraSetup>, ISimulated
{
	public void Simulate( IClient cl )
	{
		if ( Input.Pressed( "lean_left" ) )
			Lean( -1 );

		if ( Input.Pressed( "lean_right" ) )
			Lean( 1 );

		if ( Entity.GroundEntity == null && Direction != 0 )
			n_Direction = 0;
	}

	// Lean

	public int Direction => n_Direction;
	[Net] private int n_Direction { get; set; }

	public void Lean( int direction )
	{
		if ( Entity.GroundEntity == null )
		{
			return;
		}

		if ( Direction == direction )
		{
			n_Direction = 0;
			return;
		}

		n_Direction = (n_Direction + direction).Clamp( -1, 1 );
		Events.Run( new LeanDirectionChanged( n_Direction ) );
	}

	// Camera

	private float m_Distance;

	public float Distance { get; set; } = 10f;
	public float Angle { get; set; } = 10;

	public void OnPostSetup( ref CameraSetup setup )
	{
		m_Distance = m_Distance.Approach( Direction * NormalFromEyes( Direction, Entity.CollisionBounds.Translate( Entity.Position ) ), 3.5f * Time.Delta );
		var multi = 1 - setup.Hands.Aim;
		var normal = m_Distance * Easing.ExpoOut( Easing.EaseIn( m_Distance.Abs() ) );

		setup.Position += setup.Rotation.Right * Distance * normal;
		setup.Rotation *= Rotation.From( 0, 0, Angle * normal );

		setup.Hands.Angles *= Rotation.From( 0, 0, -Angle * normal );

		setup.Hands.Offset += (setup.Rotation.Right * (normal * 0.05f * Distance)) * multi;
		setup.Hands.Offset += (setup.Rotation.Down * (normal * 0.02f * Distance)) * multi;
	}

	private float NormalFromEyes( int direction, BBox bounds )
	{
		var girth = (bounds.Size.x * 2);
		var info = Trace.Ray( Entity.AimRay.Position, Entity.AimRay.Position + Entity.Rotation.Right * direction * girth )
			.Ignore( Entity )
			.Radius( 0.2f )
			.Run();

		return info.Hit ? (info.Distance - 10) / girth : 1;
	}

}
