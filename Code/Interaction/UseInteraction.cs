using Sandbox;

namespace Woosh.Espionage;

public sealed class UseInteraction : EntityComponent, IPlayerInteraction
{
	public void Simulate( TraceResult hovering, IClient client )
	{
		if ( Game.IsClient )
			return;

		using var _ = Prediction.Off();

		if ( Input.Pressed( InputButton.Use ) )
			Start( hovering );

		if ( Using == null )
			return;

		if ( !Input.Down( InputButton.Use ) )
			Stop();

		if ( Vector3.Dot( Entity.Rotation.Forward, m_UsedPosition - Entity.Position ) < 0.3f || m_UsedPosition.Distance( Entity.Position ) > 64f )
		{
			Stop();
			return;
		}

		if ( Using.OnUse( Entity ) )
		{
			Log.Info("On Use");
			return;
		}

		Stop();
	}

	public IUse Using { get; private set; }
	private Vector3 m_UsedPosition;

	private void Start( TraceResult result )
	{
		var ent = result.Entity;
		if ( ent is not IUse use || !use.IsUsable( Entity ) )
		{
			Failed();
			return;
		}

		m_UsedPosition = result.EndPosition;
		Using = use;
	}

	private void Stop()
	{
		// Call on stop here (when its supported)
		Using = null;
	}

	private void Failed()
	{
		// Play sound
	}
}
