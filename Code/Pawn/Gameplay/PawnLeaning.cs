using Sandbox;

namespace Woosh.Espionage;

public sealed partial class PawnLeaning : EntityComponent<Pawn>, IMutateCameraSetup
{
	public void Simulate( IClient cl )
	{
		if ( Input.Pressed( "lean_left" ) )
			Lean( -1 );

		if ( Input.Pressed( "lean_right" ) )
			Lean( 1 );

		if ( Entity.GroundEntity != null && Direction != 0 )
			n_Direction = 0;
	}

	// Lean

	public int Direction => n_Direction;
	[Net] private int n_Direction { get; set; }

	public void Lean( int direction )
	{
		if ( Entity.GroundEntity != null )
		{
			return;
		}

		if ( Direction == direction )
		{
			n_Direction = 0;
			return;
		}

		n_Direction = (n_Direction + direction).Clamp( -1, 1 );
	}

	// Camera

	private float m_Distance;

	public float Distance { get; set; } = 12f;
	public float Angle { get; set; } = 15;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		// Get Distance
		m_Distance = m_Distance.LerpTo( Direction * DistanceFromEyes( Direction, Entity.CollisionBounds.Translate( Entity.Position ) ), 6.5f * Time.Delta );
		m_Distance *= 1 - setup.Hands.Aim;

		setup.Position += setup.Rotation.Right * Distance * m_Distance;
		setup.Rotation *= Rotation.From( 0, 0, Angle * m_Distance );

		setup.Hands.Angles *= Rotation.From( 0, 0, -Angle * m_Distance );

		setup.Hands.Offset += setup.Rotation.Right * (m_Distance * 0.05f * Distance);
		setup.Hands.Offset += setup.Rotation.Down * (m_Distance * 0.02f * Distance);
	}

	protected float DistanceFromEyes( int direction, BBox bounds )
	{
		var girth = bounds.Size.x * 2;
		var info = Trace.Ray( Entity.AimRay.Position, Entity.AimRay.Position + Entity.Rotation.Right * direction * girth )
			.Ignore( Entity )
			.Radius( 0.2f )
			.Run();

		return info.Hit ? info.Distance / girth : 1;
	}

}
