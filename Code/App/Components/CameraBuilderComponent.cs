using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class CameraBuilderComponent : ObservableEntityComponent<App>
{
	private readonly CompositedCameraHelper m_Helper;

	public CameraBuilderComponent()
	{
		if ( Game.IsClient )
		{
			m_Helper = new CompositedCameraHelper( Camera.Main );
		}
	}

	[Listen]
	private void Update( Event<FrameUpdate> evt )
	{
		Game.AssertClient();
		using ( var mutator = m_Helper.Update( Active ) )
		{
			// Mutate Pawn
			mutator.Mutate( Game.LocalPawn as IMutate<CameraSetup> );

			// Mutate Viewmodels
		}
	}

	// Active Camera Controller

	public ICameraController Active => (ICameraController)n_Camera;
	[Net] private BaseNetworkable n_Camera { get; set; }

	public void Set<T>( T camera ) where T : BaseNetworkable, ICameraController
	{
		n_Camera = camera;
	}
}
