using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly record struct CheckAmmoOpen : IEventData { }

public readonly record struct CheckAmmoClosed : IEventData { }

public sealed partial class FirearmCheckAmmoSimulatedEntityState : ObservableEntityComponent<Firearm>, ISimulatedEntityState<Firearm>, ISingletonComponent
{
	[Predicted] private TimeSince p_SinceInput { get; set; }

	public bool TryEnter()
	{
		if ( Input.Pressed( "reload" ) )
		{
			p_SinceInput = 0;
			return false;
		}

		if ( Input.Down( "reload" ) && p_SinceInput > 0.2f )
		{
			return true;
		}

		if ( Input.Released( "reload" ) )
		{
			p_SinceInput = default;
			return false;
		}

		return false;
	}

	public bool Simulate( IClient cl )
	{
		// Check Ammo 
		return !Input.Down( "reload" );
	}

	public void OnStart()
	{
		Events.Run<CheckAmmoOpen>();
	}

	public void OnFinish()
	{
		Events.Run<CheckAmmoClosed>();
		p_SinceInput = 0;
	}
}
