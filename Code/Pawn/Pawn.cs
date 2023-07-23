using System.Linq;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public struct PawnEyes
{
	public Vector3 Position;
	public Rotation Rotation;

	public (Vector3 Position, Rotation Rotation) ToWorld( Transform transform )
	{
		return (transform.PointToWorld( Position ), transform.RotationToWorld( Rotation ));
	}
}

public partial class Pawn : ObservableAnimatedEntity, IMutate<CameraSetup>, IHave<InputContext>, IHave<IController<CameraSetup>>
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

	IController<CameraSetup> IHave<IController<CameraSetup>>.Item => Camera;

	private IController<CameraSetup> m_Camera;

	public IController<CameraSetup> Camera
	{
		get => m_Camera ?? Components.GetAny<IController<CameraSetup>>();
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

	void IMutate<CameraSetup>.OnMutate( ref CameraSetup setup )
	{
		var components = Components.All().ToArray();
		foreach ( var component in components )
		{
			if ( component is IMutate<CameraSetup> cast )
				cast.OnMutate( ref setup );
		}
	}

	// Input

	InputContext IHave<InputContext>.Item => Input;

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

		var components = Components.All().ToArray();

		// Post-Mutate Input Context
		foreach ( var component in components )
		{
			if ( component is IMutate<InputContext> cast )
				cast.OnMutate( ref context );
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

		Eyes.Position = Vector3.Up * (64f * Scale);
		Eyes.Rotation = ViewAngles.ToRotation();
		Rotation = ViewAngles.WithPitch( 0f ).ToRotation();

		if ( Machine.Simulate( cl ) )
		{
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

	public PawnEyes Eyes;

	public override Ray AimRay
	{
		get
		{
			var eyes = Eyes.ToWorld( Transform );
			return new Ray( eyes.Position, eyes.Rotation.Forward );
		}
	}

}
