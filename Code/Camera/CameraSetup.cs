using Sandbox;

namespace Woosh.Espionage;

public ref struct CameraSetup
{
	internal CameraSetup( SceneCamera camera )
		: this( camera.Position, camera.Rotation, camera.FieldOfView, camera.Size ) { }

	internal CameraSetup( Vector3 pos, Rotation rot, float fov, Vector2 viewport )
	{
		Position = pos;
		Rotation = rot;
		FieldOfView = fov;
	}

	public Vector3 Position;
	public Rotation Rotation;

	public Vector2 Viewport;
	public float FieldOfView;

	public Entity Viewer;
}
