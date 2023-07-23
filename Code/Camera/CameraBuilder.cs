using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class CameraBuilder : ObservableEntityComponent<App>
{
	public IController<CameraSetup> Active => (IController<CameraSetup>)n_Camera ?? m_Helper.Last;

	public CameraBuilder()
	{
		if ( Game.IsClient )
		{
			m_Helper = new CompositedSceneCameraHelper( Camera.Main );
		}
	}

	private readonly CompositedSceneCameraHelper m_Helper;

	[Listen]
	private void Update( Event<FrameUpdate> evt )
	{
		Game.AssertClient();

		InputContext context = default;
		if ( Game.LocalPawn is IHave<InputContext> pawn )
			context = pawn.Item;

		using ( var mutator = m_Helper.Update( context, (IController<CameraSetup>)n_Camera ) )
		{
			// Mutate Pawn
			mutator.Mutate( Game.LocalPawn as IMutate<CameraSetup> );
		}
	}

	// Camera Controller

	[Net] private BaseNetworkable n_Camera { get; set; }

	public void Apply<T>( T camera ) where T : BaseNetworkable, IController<CameraSetup>
	{
		n_Camera = camera;
	}

	public void Reset()
	{
		n_Camera = null;
	}
}
