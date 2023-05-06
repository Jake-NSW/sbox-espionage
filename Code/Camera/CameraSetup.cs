using Sandbox;

namespace Woosh.Espionage;

public ref struct CameraSetup
{
	internal CameraSetup( SceneCamera camera )
		: this( camera.Position, camera.Rotation, camera.FieldOfView ) { }

	internal CameraSetup( Vector3 pos, Rotation rot, float fov )
	{
		Position = pos;
		Rotation = rot;
		FieldOfView = fov;
	}
	
	public Entity Viewer;
	public float FieldOfView;

	public Transform Transform => new Transform( Position, Rotation );
	public Vector3 Position;
	public Rotation Rotation;

	public ViewModelSetup Hands;
}
