using System;
using Sandbox;

namespace Woosh.Espionage;

public abstract partial class CameraController<T> : CameraController where T : Entity
{
	public CameraController() : this( null ) { }
	public CameraController( T entity ) : base( entity ) { }

	protected override bool IsAttachable( Entity entity ) => entity is T;

	public new T Entity => (T)base.Entity;
}

public abstract partial class CameraController : BaseNetworkable, ICameraController
{
	public CameraController() : this( null ) { }

	public CameraController( Entity entity )
	{
		n_Entity = entity;
	}

	// State

	void ICameraController.Enabled( ref CameraSetup setup ) => Enabled( ref setup );
	void ICameraController.Update( ref CameraSetup setup, in InputContext input ) => Update( ref setup, input );
	void ICameraController.Disabled() => Disabled();

	protected virtual void Enabled( ref CameraSetup setup ) { }
	protected virtual void Update( ref CameraSetup setup, in InputContext context ) { }
	protected virtual void Disabled() { }

	// Entity

	public void AttachTo( Entity entity )
	{
		if ( n_Entity != null || !IsAttachable( entity ) )
			throw new InvalidOperationException();

		n_Entity = entity;
		n_Entity.Components.Add( new CameraHolderComponent( this ) );
	}

	protected virtual bool IsAttachable( Entity entity ) => true;

	public Entity Entity => n_Entity;
	[Net] private Entity n_Entity { get; set; }
}
