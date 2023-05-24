using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed partial class FirearmCheckAmmoSimulatedEntityState : ObservableEntityComponent<Firearm>, ISimulatedEntityState<Firearm>, ISingletonComponent
{
	private TimeSince m_SinceInput;

	public bool TryEnter()
	{
		if ( Input.Pressed( "reload" ) )
		{
			m_SinceInput = 0;
			return false;
		}

		if ( Input.Down( "reload" ) && m_SinceInput > 0.2f )
		{
			return true;
		}

		if ( Input.Released( "reload" ) )
		{
			m_SinceInput = default;
			return false;
		}

		return false;
	}

	public bool Simulate( IClient cl )
	{
		Log.Info( "checking ammo" );

		// Check Ammo 
		return !Input.Down( "reload" );
	}

	public void OnStart()
	{
		// Dispatch Checking Ammo Event

		if ( Game.IsClient )
			(Entity.Effects.Target as AnimatedEntity)?.SetAnimParameter( "bAttachmentMenu", true );
	}

	public void OnFinish()
	{
		// Dispatch Done Checking Ammo Event
		m_SinceInput = 0;
		if ( Game.IsClient )
			(Entity.Effects.Target as AnimatedEntity)?.SetAnimParameter( "bAttachmentMenu", false );
	}
}
