using Sandbox;
using Woosh.Common;
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
	private void Update( Event<PostCameraSetup> evt )
	{
		Game.AssertClient();
		InputContext context = default;

		if ( Game.LocalPawn is IHave<InputContext> pawn )
			context = pawn.Item;

		using ( var mutator = m_Helper.Update( context, Active ) )
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
