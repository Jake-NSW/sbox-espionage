using Sandbox;

namespace Woosh.Espionage;

public partial class Pawn : AnimatedEntity
{
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn()
	{
		Model = Model.Load( "models/sbox_props/watermelon/watermelon.vmdl" );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public override void OnClientActive( IClient client )
	{
		base.OnClientActive( client );
		Log.Info(client.Name);
	}
	
	public override void ClientSpawn()
	{
		base.ClientSpawn();

	//	/*
		var viewModel = new CompositedViewModel( null ) { Owner = this, Model = Model.Load( "weapons/mk23/v_espionage_mk23.vmdl" ) };

		viewModel.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
		viewModel.Add( new ViewModelSwayEffect() );
		viewModel.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
		viewModel.Add( new ViewModelStrafeOffsetEffect() { Damping = 6, RollMultiplier = 1, AxisMultiplier = 8 } );
		viewModel.Add( new ViewModelDeadzoneSwayEffect( new Vector2( 8, 8 ) ) { AimingOnly = true, AutoCenter = false, Damping = 8 } );
		viewModel.Add( new ViewModelPitchOffsetEffect() );

		viewModel.SetAnimParameter( "bDeployed", true );
		viewModel.SetBodyGroup( "Muzzle", 1 );
		viewModel.SetBodyGroup( "Module", 1 );
		// */

		/*
		var viewModel2 = new CompositedViewModel( null ) { Owner = this, Model = Model.Load( "weapons/smg2/v_espionage_smg2.vmdl" ) };
		viewModel2.SetAnimParameter( "bDeployed", true );
		viewModel2.SetBodyGroup( "Muzzle", 1 );
		viewModel2.Add( new ViewModelSwayEffect() { } );
		viewModel2.Add( new ViewModelPitchOffsetEffect(3, 2)  );
		viewModel2.Add( new ViewModelOffsetEffect(new Vector3(-2, 0, 0), Vector3.Zero) { } );
		viewModel2.Add( new ViewModelDeadzoneSwayEffect(new Vector2(8)) { } );
		*/
	}

	// An example BuildInput method within a player's Pawn class.
	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Angles ViewAngles { get; set; }

	public override void BuildInput()
	{
		InputDirection = Input.AnalogMove;

		var look = Input.AnalogLook;

		var viewAngles = ViewAngles;
		viewAngles += look;
		ViewAngles = viewAngles.Normal;
	}

	/// <summary>
	/// Called every tick, clientside and serverside.
	/// </summary>
	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		Components.GetOrCreate<InteractionHandler>().Simulate( cl );

		Rotation = ViewAngles.ToRotation();

		// build movement from the input values
		var movement = InputDirection.Normal;

		// rotate it to the direction we're facing
		Velocity = Rotation * movement;

		// apply some speed to it
		Velocity *= Input.Down( InputButton.Run ) ? 1000 : 200;

		// apply it to our position using MoveHelper, which handles collision
		// detection and sliding across surfaces for us
		MoveHelper helper = new MoveHelper( Position, Velocity );
		helper.Trace = helper.Trace.Size( 16 );
		if ( helper.TryMove( Time.Delta ) > 0 )
		{
			Position = helper.Position;
		}

		// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
		if ( Game.IsServer && Input.Pressed( InputButton.PrimaryAttack ) )
		{
			var ragdoll = new Prop();
			ragdoll.Model = Model.Load( "models/sbox_props/watermelon/watermelon.vmdl" );
			ragdoll.Position = Position + Rotation.Forward * 40;
			ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
			ragdoll.PhysicsGroup.Velocity = Rotation.Forward * 1000;
			ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		}
	}

	/// <summary>
	/// Called every frame on the client
	/// </summary>
	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );

		// Update rotation every frame, to keep things smooth
		Rotation = ViewAngles.ToRotation();

		Camera.Position = Position;
		Camera.Rotation = Rotation;

		// Set field of view to whatever the user chose in options
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );

		// Set the first person viewer to this, so it won't render our model
		Camera.FirstPersonViewer = this;
	}
}
