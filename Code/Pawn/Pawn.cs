using System.Linq;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public partial class Pawn : ObservableAnimatedEntity, IHave<InputContext>, IPostMutate<CameraSetup>
{
	public EntityStateMachine<Pawn> Machine { get; }

	protected Pawn()
	{
		Machine = new EntityStateMachine<Pawn>( this );
	}

	public override void Spawn()
	{
		Model = Model.Load( "models/sbox_props/watermelon/watermelon.vmdl" );

		Tags.Add( "pawn" );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	// Camera

	private IController<CameraSetup> m_Camera;

	public IController<CameraSetup> Camera
	{
		get
		{
			if ( m_Camera != null )
				return m_Camera;

			return Components.GetAny<IController<CameraSetup>>();
		}
		set
		{
			if ( m_Camera != value && m_Camera is IComponent oldComponent )
			{
				// Remove camera from component list?
				Components.Remove( oldComponent );
			}

			m_Camera = value;

			if ( m_Camera is IComponent newComponent )
			{
				// Add camera to component list?
				Components.Add( newComponent );
			}
		}
	}

	void IPostMutate<CameraSetup>.OnPostMutate( ref CameraSetup setup )
	{
		var components = Components.All().ToArray();

		foreach ( var component in components )
		{
			if ( component is IPreMutate<CameraSetup> cast )
				cast.OnPreMutate( ref setup );
		}

		foreach ( var component in components )
		{
			if ( component is IPostMutate<CameraSetup> cast )
				cast.OnPostMutate( ref setup );
		}
	}

	// Input

	InputContext IHave<InputContext>.Item => new InputContext()
	{
		InputDirection = InputDirection,
		ViewAngles = ViewAngles,
		Muzzle = Muzzle
	};

	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Angles ViewAngles { get; set; }
	[ClientInput] public Ray Muzzle { get; set; }

	public override sealed void BuildInput()
	{
		var context = InputContext.FromViewAngles( ViewAngles );
		var components = Components.All().ToArray();

		// Pre-Mutate Input Context
		foreach ( var component in components )
		{
			if ( component is IPreMutate<InputContext> cast )
				cast.OnPreMutate( ref context );
		}

		// Post-Mutate Input Context
		foreach ( var component in components )
		{
			if ( component is IPostMutate<InputContext> cast )
				cast.OnPostMutate( ref context );
		}

		InputDirection = context.InputDirection;
		ViewAngles = context.ViewAngles;
		Muzzle = context.Muzzle;
	}

	// Simulate

	private IClient m_Last;

	public override void Simulate( IClient cl )
	{
		if ( m_Last != Client )
		{
			// Dispatch On Pawn Registered
			Events.Run( new EntityUnPossessed(), Propagation.Trickle );
			m_Last = Client;
			Events.Run( new EntityPossessed( cl ), Propagation.Trickle );
		}

		EyeRotation = ViewAngles.ToRotation();
		Rotation = ViewAngles.WithPitch( 0f ).ToRotation();
		EyeLocalPosition = Vector3.Up * (64f * Scale);

		if ( Machine.Simulate( cl ) )
		{
			Components.Each( cl, ( IClient client, ISimulated e ) => e.Simulate( client ) );
		}
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		Rotation = ViewAngles.WithPitch( 0f ).ToRotation();
	}

	public virtual BBox Hull => new BBox( new Vector3( -10, -10, 0 ), new Vector3( 10, 10, 64 ) );

	// Eyes

	public override Ray AimRay => new Ray( EyePosition, EyeRotation.Forward );

	public Vector3 EyePosition
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	[Net, Predicted] public Vector3 EyeLocalPosition { get; set; }

	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	[Net, Predicted] public Rotation EyeLocalRotation { get; set; }

}
