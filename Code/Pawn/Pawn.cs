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
		Components.Create<FlyPawnController>();
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

		Rotation = ViewAngles.ToRotation();

		Components.Get<InteractionHandler>()?.Simulate( cl );
		Components.Get<CarriableHandler>()?.Simulate( cl );
		Components.Get<PawnController>()?.Simulate( cl );

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
