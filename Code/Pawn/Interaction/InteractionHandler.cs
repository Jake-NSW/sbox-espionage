using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly struct InteractionTargetChanged : IEventData
{
	public Entity Hovering { get; }
	public IReadOnlyList<IEntityInteraction> Interactions { get; }

	public InteractionTargetChanged( Entity hovering, IReadOnlyList<IEntityInteraction> interactions )
	{
		Hovering = hovering;
		Interactions = interactions;
	}
}

public sealed class InteractionHandler : ObservableEntityComponent<Pawn>, ISingletonComponent
{
	public InteractionHandler()
	{
		m_Interactions = Array.Empty<IEntityInteraction>();
	}

	// State

	public IReadOnlyList<IEntityInteraction> Interactions => m_Interactions;
	private IEntityInteraction[] m_Interactions;

	public Entity Hovering => p_Hovering;
	[Predicted] private Entity p_Hovering { get; set; }

	public void Simulate( IClient client )
	{
		var result = Scan();
		var hovering = result.Entity;

		if ( p_Hovering != hovering )
		{
			p_Hovering = hovering;
			Rebuild();
		}

		foreach ( var interaction in Interactions )
		{
			interaction.Simulate( result, client );
		}
	}

	public void Rebuild()
	{
		m_Interactions = p_Hovering == null ? Array.Empty<IEntityInteraction>() : Entity.Components.GetAll<IEntityInteraction>().Where( e => e.IsInteractable( p_Hovering ) ).ToArray();
		Events.Run( new InteractionTargetChanged( p_Hovering, m_Interactions ) );
	}

	private TraceResult Scan( float size = 8 )
	{
		var eyes = Entity.AimRay;
		var ray = Trace.Ray( eyes.Position, eyes.Position + eyes.Forward * 48 ).Ignore( Entity ).EntitiesOnly();

		var first = ray.Run();
		return first.Entity != null ? first : ray.Size( size ).Run();
	}
}
