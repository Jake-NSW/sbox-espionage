using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class FallPawnState : ObservableEntityComponent<Pawn>, IEntityState<Pawn>
{
	public bool TryEnter()
	{
		return Input.Pressed( App.Actions.Fall );
	}

	[Net, Predicted] private TimeSince n_SinceEntered { get; set; }

	public bool Simulate( IClient cl )
	{
		if ( Input.Pressed( App.Actions.Jump ) && n_SinceEntered > 1.9f )
		{
			return true;
		}

		return false;
	}

	public void OnStart()
	{
		Log.Info( "Entering" );

		n_SinceEntered = 0;
		Entity.Camera = new RagDollCamera();

		{
			Entity.UsePhysicsCollision = true;
			Entity.PhysicsEnabled = true;
			Entity.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		}
	}

	public void OnFinish()
	{
		Log.Info( "Leaving!!!!" );
		Entity.Camera = null;

		{
			Entity.PhysicsEnabled = false;
			Entity.UsePhysicsCollision = false;
			Entity.PhysicsClear();
		}
	}
}
