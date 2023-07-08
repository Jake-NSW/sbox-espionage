using Sandbox;

namespace Woosh.Espionage;

// Controller

public abstract class EntityCameraController<T> : EntityCameraController where T : Entity
{
	public new T Entity => base.Entity as T;

	public override bool CanAddToEntity( Entity entity )
	{
		return entity is T;
	}
}

public abstract class EntityCameraController : EntityComponent, ICameraController, ISingletonComponent
{
	void ICameraController.Enabled( ref CameraSetup setup ) => OnEnabled( ref setup );
	protected virtual void OnEnabled( ref CameraSetup setup ) { }

	void ICameraController.Update( ref CameraSetup setup, in InputContext input ) => OnUpdate( ref setup, input );
	protected abstract void OnUpdate( ref CameraSetup setup, in InputContext input );

	void ICameraController.Disabled() => OnDisabled();
	protected virtual void OnDisabled() { }
}

public interface ICameraController
{
	void Enabled( ref CameraSetup setup );
	void Update( ref CameraSetup setup, in InputContext input );
	void Disabled();
}
