using Sandbox;

namespace Woosh.Espionage;

public sealed class InteractionHandler : EntityComponent, ISingletonComponent
{
	public StructEventDispatcher Events { get; }

	public InteractionHandler()
	{
		Events = new StructEventDispatcher();
	}

	public Entity Hovering => p_Last;
	[Predicted] private Entity p_Last { get; set; }

	public void Simulate( IClient client )
	{
		var result = Scan();
		p_Last = result.Entity;
		
		foreach ( var interaction in Entity.Components.GetAll<IEntityInteraction>() )
		{
			interaction.Simulate( result, client );
		}
	}

	private TraceResult Scan( float size = 8 )
	{
		var eyes = Entity.AimRay;
		var ray = Trace.Ray( eyes.Position, eyes.Position + eyes.Forward * 48 ).Ignore( Entity ).EntitiesOnly();

		var first = ray.Run();
		return first.Entity != null ? first : ray.Size( size ).Run();
	}
}
