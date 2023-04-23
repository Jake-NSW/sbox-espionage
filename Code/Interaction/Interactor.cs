using System.Linq;
using Sandbox;

namespace Woosh.Espionage.Interaction;

public sealed class Interactor : EntityComponent, ISingletonComponent
{
	public Entity Hovering { get; private set; }

	public void Simulate( IClient client )
	{
		var result = Scan();
		Hovering = result.Entity;
		foreach ( var interaction in Entity.Components.GetAll<EntityComponent>().OfType<IPlayerInteraction>() )
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
