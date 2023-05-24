using System.Collections.Generic;
using Sandbox;

namespace Woosh.Espionage;

public struct CameraSetup
{
	public CameraSetup( CameraSetup setup ) : this( setup.Position, setup.Rotation, setup.FieldOfView )
	{
		Hands = setup.Hands;
		Effects = setup.Effects;
		Viewer = setup.Viewer;
	}

	public CameraSetup( SceneCamera camera )
		: this( camera.Position, camera.Rotation, camera.FieldOfView ) { }

	public CameraSetup( Vector3 pos, Rotation rot, float fov )
	{
		Position = pos;
		Rotation = rot;
		FieldOfView = fov;
		Hands = new ViewModelSetup();

		Effects = new HashSet<ITemporaryCameraEffect>();
	}

	public HashSet<ITemporaryCameraEffect> Effects { get; }

	public static CameraSetup Lerp( CameraSetup from, CameraSetup to, float t )
	{
		return new CameraSetup(
			Vector3.Lerp( from.Position, to.Position, t ),
			Rotation.Lerp( from.Rotation, to.Rotation, t ),
			MathX.Lerp( from.FieldOfView, to.FieldOfView, t )
		) { Hands = ViewModelSetup.Lerp( from.Hands, to.Hands, t ) };
	}

	public Entity Viewer;
	public float FieldOfView;

	public Transform Transform => new Transform( Position, Rotation );
	public Vector3 Position;
	public Rotation Rotation;

	public ViewModelSetup Hands;
}
