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

	protected override void OnAutoRegister()
	{
		if ( Game.IsClient )
		{
			Register<FrameUpdate>( Update );
		}
	}

	private void Update( Event<FrameUpdate> evt )
	{
		Game.AssertClient();
		m_Helper.Update( mutate: Game.LocalPawn as IMutate<CameraSetup> );
	}

	public void AddEffect( ITemporaryCameraEffect effect )
	{
		m_Helper.Effects.Add( effect );
	}

	public void RemoveEffect( ITemporaryCameraEffect effect )
	{
		m_Helper.Effects.Remove( effect );
	}

	// Active Camera Controller

	[Net] private BaseNetworkable n_Camera { get; set; }

	public void Set<T>( T camera ) where T : BaseNetworkable, ICameraController
	{
		n_Camera = camera;
	}
}
