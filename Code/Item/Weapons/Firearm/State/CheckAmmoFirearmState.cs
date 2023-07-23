using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class CheckAmmoFirearmState : ObservableEntityComponent<Firearm>, IEntityState<Firearm>, ISingletonComponent
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
		Run<CheckAmmoOpen>( propagation: Propagation.Bubble );
	}

	public void OnFinish()
	{
		Run<CheckAmmoClosed>( propagation: Propagation.Bubble );
		p_SinceInput = 0;
	}
}
