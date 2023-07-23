using Sandbox;

namespace Woosh.Espionage;

/// <summary>
/// The CameraSetup is responsible for being able to control a <see cref="SceneCamera"/> over a given frame. We use CameraSetup
/// for mutating the camera in a particular way. This is useful for things like first person cameras, third person cameras, and
/// being able to add further camera effects on top of the base camera controller.
/// </summary>
public struct CameraSetup
{
	/// <summary>
	/// Creates a new CameraSetup from a SceneCamera. This will copy over the render tags, exclude tags, render attributes
	/// and will not directly mutate the SceneCamera.
	/// </summary>
	public CameraSetup( SceneCamera camera ) : this( camera.RenderTags, camera.ExcludeTags, camera.Attributes )
	{
		Viewer = camera.FirstPersonViewer;
		Position = camera.Position;
		Rotation = camera.Rotation;
		FieldOfView = camera.FieldOfView;
	}

	private CameraSetup( CameraSetup setup ) : this( setup.Include, setup.Exclude, setup.Attributes )
	{
		Hands = setup.Hands;
		Viewer = setup.Viewer;
	}

	private CameraSetup( ITagSet include, ITagSet exclude, RenderAttributes attributes )
	{
		Attributes = attributes;
		Include = include;
		Exclude = exclude;
	}

	/// <summary>
	/// Interpolate between two CameraSetups. This is incredibly useful for ViewModel and Camera effects.
	/// <exception cref="System.Exception"> Not targeting the same scene camera... </exception>
	/// </summary>
	public static CameraSetup Lerp( CameraSetup from, CameraSetup to, float t )
	{
		if ( to.Attributes != from.Attributes )
			throw new System.Exception( "Cannot lerp between two different scene cameras" );

		if ( to.Viewer != from.Viewer )
			throw new System.Exception( "Cannot lerp between two different viewers" );

		return new CameraSetup( from )
		{
			FieldOfView = MathX.LerpTo( from.FieldOfView, to.FieldOfView, t ),
			Position = Vector3.Lerp( from.Position, to.Position, t ),
			Rotation = Rotation.Lerp( from.Rotation, to.Rotation, t ),
			Hands = ViewModelSetup.Lerp( from.Hands, to.Hands, t )
		};
	}

	/// <summary>
	/// Render Attributes that are on the SceneCamera that we are mutating.
	/// </summary>
	public RenderAttributes Attributes { get; }

	/// <summary>
	/// Include Render Tags that are on the SceneCamera that we are mutating.
	/// </summary>
	public ITagSet Include { get; }

	/// <summary>
	/// Exclude Render Tags that are on the SceneCamera that we are mutating.
	/// </summary>
	public ITagSet Exclude { get; }

	/// <summary>
	/// The current first person viewer, that is using this camera to look through/
	/// </summary>
	public IEntity Viewer;

	/// <summary>
	/// The current field of view of the SceneCamera that we are mutating.
	/// </summary>
	public float FieldOfView;

	/// <summary>
	/// A helper for getting the transform off this CameraSetup
	/// </summary>
	public Transform Transform => new Transform( Position, Rotation );

	/// <summary>
	/// The active Position of the camera in world space
	/// </summary>
	public Vector3 Position;

	/// <summary>
	/// The rotation of the camera in world space
	/// </summary>
	public Rotation Rotation;

	/// <summary>
	/// Further setup for the hands that are being rendered, if any.
	/// </summary>
	public ViewModelSetup Hands;
}
