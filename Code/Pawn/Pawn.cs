using Sandbox;

namespace Woosh.Espionage;

public partial class Pawn : AnimatedEntity
{
	public override void Spawn()
	{
		Model = Model.Load( "models/sbox_props/watermelon/watermelon.vmdl" );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Components.Create<FirstPersonEntityCamera>();
	}

	// Input

	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Angles ViewAngles { get; set; }

	public override void BuildInput()
	{
		InputDirection = Input.AnalogMove;
		ViewAngles = (ViewAngles + Input.AnalogLook).Normal;
		
		Components.Get<FirstPersonEntityCamera>()?.Feed( new InputContext( ViewAngles ) );
	}

	// Simulate

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		Components.GetOrCreate<InteractionHandler>().Simulate( cl );
		Components.GetOrCreate<CarriableHandler>().Simulate( cl );

		Rotation = ViewAngles.ToRotation();

		// build movement from the input values
		var movement = InputDirection.Normal;

		// rotate it to the direction we're facing
		Velocity = Rotation * movement;

		// apply some speed to it
		Velocity *= Input.Down( "run" ) ? 1000 : 200;

		// apply it to our position using MoveHelper, which handles collision
		// detection and sliding across surfaces for us
		MoveHelper helper = new MoveHelper( Position, Velocity );
		helper.Trace = helper.Trace.Size( 16 );
		if ( helper.TryMove( Time.Delta ) > 0 )
		{
			Position = helper.Position;
		}

		// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
		if ( Game.IsServer && Input.Pressed( "shoot" ) )
		{
			_ = new Prop
			{
				Model = Model.Load( "models/sbox_props/watermelon/watermelon.vmdl" ),
				Position = Position + Rotation.Forward * 40,
				Rotation = Rotation.LookAt( Vector3.Random.Normal ),
				Scale = 0.4f,
				PhysicsGroup = { Velocity = Rotation.Forward * 1000 }
			};
		}
	}
}
