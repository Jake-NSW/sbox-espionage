using Sandbox;
using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public abstract class PawnHudComponent<T> : ObservableEntityComponent<Pawn> where T : RootPanel, new()
{
	protected override void OnAutoRegister()
	{
		if ( Game.IsClient )
		{
			// Only care on the Client, as its UI
			Register<EntityPossessed>( OnPawnPossessed );
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Game.IsClient && Game.LocalPawn == Entity )
		{
			OnCreateUI( m_Panel = new T() );
		}
	}

	private T m_Panel;

	private void OnPawnPossessed( Event<EntityPossessed> signal )
	{
		// Delete Old UI
		m_Panel?.Delete();

		if ( signal.Data.Client != Game.LocalClient )
			return;

		OnCreateUI( m_Panel = new T() );
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		m_Panel?.Delete();
	}

	protected virtual void OnCreateUI( T root ) { }
}
