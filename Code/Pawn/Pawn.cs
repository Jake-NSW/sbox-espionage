using System.Linq;
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

	// Simulate

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		Rotation = ViewAngles.ToRotation();
		Components.Get<PawnController>()?.Simulate( cl );
	}
}
