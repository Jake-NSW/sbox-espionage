using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class CameraBuilderComponent : ObservableEntityComponent<App>
{
	public CameraBuilderComponent()
	{
		if ( Game.IsClient )
		{
			m_Helper = new CompositedCameraHelper( Sandbox.Camera.Main );
		}
	}

	private readonly CompositedCameraHelper m_Helper;

	[Listen]
	private void Update( Event<FrameUpdate> evt )
	{
		Game.AssertClient();

		InputContext context = default;
		if ( Game.LocalPawn is IHave<InputContext> pawn )
			context = pawn.Item;

		using ( var mutator = m_Helper.Update( context, Active ) )
		{
			// Mutate Pawn
			mutator.Mutate( Game.LocalPawn as IPostMutate<CameraSetup> );

			// Mutate Other Stuff?
		}
	}

	// Active Camera Controller

	public ICameraController Active => (ICameraController)n_Camera;
	[Net] private BaseNetworkable n_Camera { get; set; }

	public void Apply<T>( T camera ) where T : BaseNetworkable, ICameraController
	{
		n_Camera = camera;
	}

	public void Reset()
	{
		n_Camera = null;
	}
}
