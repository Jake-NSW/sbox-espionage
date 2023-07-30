using System.Linq;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

/// <summary>
/// A "Pawn Entity" is meant to represent a controllable character. It is used to represent the player, NPCs, and other controllable
/// entities. Pawns are assigned to the client that controls them. They are given a camera controller, input and are simulated by
/// default (when possessed). They are also given a state machine to control their state.
/// </summary>
public partial class Pawn : ObservableAnimatedEntity, IMutate<CameraSetup>, IHave<InputContext>, IHave<IController<CameraSetup>>
{
	/// <summary>
	/// The state machine that controls the pawn's state. This is used to control the pawn's behavior. This does not control the pawn's
	/// character controller. As that is active when there is no state. 
	/// </summary>
	public EntityStateMachine<Pawn> Machine { get; }

	public Pawn()
	{
		Machine = new EntityStateMachine<Pawn>( this );
	}

	public override void Spawn()
	{
		Model = Model.Load( "models/citizen/citizen.vmdl" );

		Tags.Add( "pawn" );

		EnableDrawing = true;
		EnableHitboxes = true;
		EnableLagCompensation = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	// Camera

	IController<CameraSetup> IHave<IController<CameraSetup>>.Item => Camera;

	private IController<CameraSetup> n_Camera { get; set; }

	/// <summary>
	/// The camera controller that controls the pawn's camera. It is only used when the pawn is possessed by the local client, or is being
	/// spectated. If the inputted camera is a component, it will be added to the pawn's component list.
	/// </summary>
	public IController<CameraSetup> Camera
	{
		get => n_Camera ?? Components.GetAny<IController<CameraSetup>>();
		set
		{
			if ( n_Camera != value && n_Camera is IComponent oldComponent )
			{
				// Remove camera from component list?
				Components.Remove( oldComponent );
			}

			n_Camera = value;

			if ( n_Camera is IComponent newComponent )
			{
				// Add camera to component list?
				Components.Add( newComponent );
			}
		}
	}

	void IMutate<CameraSetup>.OnMutate( ref CameraSetup setup )
	{
		PreCameraSetup( ref setup );

		var components = Components.All().ToArray();
		foreach ( var component in components )
		{
			if ( component is IMutate<CameraSetup> cast )
				cast.OnMutate( ref setup );
		}

		PostCameraSetup( ref setup );
	}

	/// <summary>
	/// Used to mutate the camera setup before it is used. This is called before any components mutators are called.
	/// </summary>
	protected virtual void PreCameraSetup( ref CameraSetup setup ) { }

	/// <summary>
	/// Used to mutate the camera setup after it is used. This is called after all components mutators are called.
	/// </summary>
	protected virtual void PostCameraSetup( ref CameraSetup setup ) { }

	// Input

	InputContext IHave<InputContext>.Item => Input;

	/// <summary>
	/// The pawns current input context. This is used to control the pawn's character controller and the camera controller. This is meant
	/// to be provided as a nice way to access pawn specific input. 
	/// </summary>
	public InputContext Input => new InputContext()
	{
		InputDirection = InputDirection,
		ViewAngles = ViewAngles,
		Muzzle = Muzzle
	};

	[ClientInput, System.Obsolete( "This is incredibly hacky. Why doesn't sbox have a good way to handle this.." )]
	public Ray Muzzle { get; set; }

	[ClientInput] public Vector3 InputDirection { get; set; }
	[ClientInput] public Angles ViewAngles { get; set; }

	public override sealed void BuildInput()
	{
		var context = new InputContext
		{
			InputDirection = Sandbox.Input.AnalogMove,
			ViewAngles = (ViewAngles + Sandbox.Input.AnalogLook).Normal
		};

		PreInputContext( ref context );

		var components = Components.All().ToArray();

		// Post-Mutate Input Context
		foreach ( var component in components )
		{
			if ( component is IMutate<InputContext> cast )
				cast.OnMutate( ref context );
		}

		PostInputContext( ref context );

		InputDirection = context.InputDirection;
		ViewAngles = context.ViewAngles;
		Muzzle = context.Muzzle;
	}

	protected virtual void PreInputContext( ref InputContext context ) { }
	protected virtual void PostInputContext( ref InputContext context ) { }

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

		if ( Machine.Simulate( cl ) && IsAuthority )
		{
			EyeRotation = ViewAngles.ToRotation();
			EyeLocalPosition = Vector3.Up * (64f * Scale);
			Rotation = ViewAngles.WithPitch( 0f ).ToRotation();

			Components.Get<PawnController>().Simulate( cl );
			base.Simulate( cl );
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
