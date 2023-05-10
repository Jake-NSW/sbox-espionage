using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class InteractionHandler : EntityComponent, ISingletonComponent
{
	public Dispatcher Events { get; }

	public InteractionHandler()
	{
		Events = new Dispatcher();
		m_Interactions = Array.Empty<IEntityInteraction>();
	}

	// State

	public IReadOnlyList<IEntityInteraction> Interactions => m_Interactions;
	private IEntityInteraction[] m_Interactions;

	public Entity Hovering => p_Last;
	[Predicted] private Entity p_Last { get; set; }

	private int m_Length;
	
	public void Simulate( IClient client )
	{
		var result = Scan();
		var hovering = result.Entity;

		if ( p_Last != hovering || m_Length != Entity.Components.Count)
		{
			m_Length = Entity.Components.Count;
			
			var old = p_Last;
			p_Last = hovering;
			Rebuild();
			
			Events.Notify( p_Last, old );
		}

		foreach ( var interaction in Interactions )
		{
			interaction.Simulate( result, client );
		}
	}

	public void Rebuild()
	{
		m_Interactions = p_Last == null ? Array.Empty<IEntityInteraction>() : Entity.Components.GetAll<IEntityInteraction>().Where( e => e.IsInteractable( p_Last ) ).ToArray();
	}

	private TraceResult Scan( float size = 8 )
	{
		var eyes = Entity.AimRay;
		var ray = Trace.Ray( eyes.Position, eyes.Position + eyes.Forward * 48 ).Ignore( Entity ).EntitiesOnly();

		var first = ray.Run();
		return first.Entity != null ? first : ray.Size( size ).Run();
	}
}
