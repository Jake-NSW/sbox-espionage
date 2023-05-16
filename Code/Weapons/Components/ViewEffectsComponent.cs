using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class ViewEffectsComponent : ObservableEntityComponent<ObservableAnimatedEntity>
{
	public AnimatedEntity Effects => Entity;

	private AnimatedEntity m_Viewmodel;

	[Auto]
	private void OnHolstered( in Event<HolsteredEntity> evt )
	{
		m_Viewmodel?.Delete();
		m_Viewmodel = null;

		// Delete View Model
		Effects.EnableDrawing = false;
	}

	[Auto]
	private void Deploying( in Event<DeployingEntity> evt )
	{
		if ( Entity.IsLocalPawn )
		{
			// Create View Model... But how?!
		}

		Effects.EnableDrawing = true;
	}
}
