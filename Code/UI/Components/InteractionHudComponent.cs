﻿using Sandbox.UI;
using Woosh.Espionage.UI;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class InteractionHudComponent : EntityHudComponent<RootPanel, Pawn>
{
	protected override void OnAutoRegister()
	{
		base.OnAutoRegister();
		
		Register<InteractionTargetChanged>( OnInteractionChanged );
	}

	private void OnInteractionChanged( Event<InteractionTargetChanged> evt )
	{
		m_Interaction?.OnInteractionChanged( evt );
	}

	private Interaction m_Interaction;

	protected override void OnCreateUI( RootPanel root )
	{
		root.AddChild( m_Interaction = new Interaction() );
	}

}