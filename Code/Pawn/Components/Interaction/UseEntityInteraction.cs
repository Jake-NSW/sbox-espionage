using Sandbox;

namespace Woosh.Espionage;

public sealed class UseEntityInteraction : EntityComponent<Pawn>, IEntityInteraction, ISingletonComponent
{
	public InteractionIndicator Indicator => new InteractionIndicator( App.Actions.Interact, Input.GetButtonOrigin( App.Actions.Interact ), 0 );

	public bool IsInteractable( Entity entity )
	{
		return entity is IUse;
	}

	public void Simulate( in TraceResult context, IClient client )
	{
		if ( Game.IsClient )
			return;

		using var _ = Prediction.Off();

		if ( Input.Pressed( App.Actions.Interact ) )
			Start( context );

		if ( Using == null )
			return;

		if ( !Input.Down( App.Actions.Interact ) )
			Stop();

		if ( Using.OnUse( Entity ) )
			return;

		Stop();
	}

	public IUse Using { get; private set; }

	private void Start( TraceResult context )
	{
		var ent = context.Entity;
		if ( ent is not IUse use || !use.IsUsable( Entity ) )
		{
			Failed();
			return;
		}
		
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
		Log.Info( "Failed Use" );
	}
}
