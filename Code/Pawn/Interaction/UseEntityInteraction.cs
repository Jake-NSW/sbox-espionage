using Sandbox;

namespace Woosh.Espionage;

public sealed class UseEntityInteraction : EntityComponent, IEntityInteraction, ISingletonComponent
{
	public InteractionIndicator Indicator => new InteractionIndicator( "Use", Input.GetButtonOrigin( "use" ), 0 );

	public bool IsInteractable( Entity entity )
	{
		return entity is IUse;
	}

	public void Simulate( in TraceResult context, IClient client )
	{
		if ( Game.IsClient )
			return;

		using var _ = Prediction.Off();

		if ( Input.Pressed( "use" ) )
			Start( context );

		if ( Using == null )
			return;

		if ( !Input.Down( "use" ) )
			Stop();

		if ( Vector3.Dot( Entity.Rotation.Forward, m_UsedPosition - Entity.Position ) < 0.3f || m_UsedPosition.Distance( Entity.Position ) > 64f )
		{
			Stop();
			return;
		}

		if ( Using.OnUse( Entity ) )
		{
			return;
		}

		Stop();
	}

	public IUse Using { get; private set; }
	private Vector3 m_UsedPosition;

	private void Start( TraceResult context )
	{
		var ent = context.Entity;
		if ( ent is not IUse use || !use.IsUsable( Entity ) )
		{
			Failed();
			return;
		}

		m_UsedPosition = context.EndPosition;
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
