using System.ComponentModel;
using System.Linq;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public partial class Pawn : AnimatedEntity, IObservableEntity
{
	public IDispatcher Events { get; } = new Dispatcher();
	public EntityStateMachine<Pawn> Machine { get; }

	public Pawn()
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

		Components.Create<FirstPersonEntityCamera>();
	}

	// Input

	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Angles ViewAngles { get; set; }

	public override sealed void BuildInput()
	{
		var context = new InputContext() { InputDirection = Input.AnalogMove, ViewAngles = (ViewAngles + Input.AnalogLook).Normal };
		OnPostInputBuild( ref context );

		foreach ( var input in Components.All().OfType<IMutateInputContext>() )
		{
			// Post Build Input
			input.OnPostInputBuild( ref context );
		}

		InputDirection = context.InputDirection;
		ViewAngles = context.ViewAngles;
	}

	protected virtual void OnPostInputBuild( ref InputContext context ) { }

	// Components

	protected override void OnComponentAdded( EntityComponent component )
	{
		base.OnComponentAdded( component );

		Events.Run( new ComponentAdded( component ) );
	}

	// Simulate

	public BBox Hull => new BBox( new Vector3( -10, -10, 0 ), new Vector3( 10, 10, 64 ) );


	/// <summary>
	/// Position a player should be looking from in world space.
	/// </summary>
	[Browsable( false )]
	public Vector3 EyePosition
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	/// <summary>
	/// Position a player should be looking from in local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Vector3 EyeLocalPosition { get; set; }

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity.
	/// </summary>
	[Browsable( false )]
	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Rotation EyeLocalRotation { get; set; }
	public override Ray AimRay => new Ray( EyePosition, EyeRotation.Forward );
	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		EyeRotation = ViewAngles.ToRotation();
		Rotation = ViewAngles.WithPitch( 0f ).ToRotation();

		if ( Machine.Simulate( cl ) )
		{
			Components.Get<PawnController>()?.Simulate( cl );
		}

		EyeLocalPosition = Vector3.Up * (64f * Scale);
	}
}
