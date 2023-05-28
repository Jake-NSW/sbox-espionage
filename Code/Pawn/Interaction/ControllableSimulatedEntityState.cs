using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ControllableSimulatedEntityState : ObservableEntityComponent<Pawn>, ISimulatedEntityState<Pawn>,
	ISingletonComponent, IMutate<CameraSetup>
{
	protected override void OnAutoRegister()
	{
		Register<InteractionTargetChanged>( evt => m_Target = evt.Data.Hovering );
	}

	private Entity m_Target;

	public bool TryEnter()
	{
		return Input.Pressed( "use" ) && m_Target is IControllable;
	}

	public bool Simulate( IClient cl )
	{
		// Tab to Leave? Or would it be use?
		return (m_Target is IControllable controllable) && controllable.Simulate( Entity );
	}

	public void OnStart()
	{
		// Tell Entity we are Controlling it
		(m_Target as IControllable)?.Entering( Entity );
	}

	public void OnFinish()
	{
		(m_Target as IControllable)?.Leaving();
	}

	public void OnPostSetup( ref CameraSetup setup )
	{
		if ( Entity.Machine.Active == this )
			(m_Target as IMutate<CameraSetup>)?.OnPostSetup( ref setup );
	}
}
