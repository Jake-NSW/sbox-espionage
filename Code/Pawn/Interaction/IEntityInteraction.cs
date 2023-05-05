using System;
using Sandbox;

namespace Woosh.Espionage;

public struct InteractionIndicator
{
	public InteractionIndicator( string action, string bind, float held )
	{
		Action = action;
		Bind = bind;
		Held = MathF.Min( 1, held );
	}

	public string Action { get; }
	public string Bind { get; }
	public float Held { get; }
}

/// <summary>
/// An Entity Interaction is a Component that is attached to possessing pawn that gives it the ability
/// to define new interaction logic, where the heavy lifting is done by the <see cref="InteractionHandler"/>
/// </summary>
public interface IEntityInteraction : IComponent
{
	/// <summary>
	/// Used by the UI to indicate some sort of interaction. Is meant to tell the UI how long this
	/// interaction  has been used, the keybinding required to invoke it and any action details
	/// about it to provide context to the player.
	/// </summary>
	public InteractionIndicator Indicator { get; }

	/// <summary>
	/// Is Interactable is called when the currently hovering item is changed on the Interaction
	/// handler. This allows us to inject more interaction logic into the handler. This controls
	/// whether or not we should simulate the component or not...
	/// <param name="entity"> The currently hovering entity. </param>
	/// <returns> True if we should be simulating. </returns>
	/// </summary>
	bool IsInteractable( Entity entity );

	/// <summary>
	/// Called every simulate if the currently hovering entity is able to be interacted. Which is
	/// defined by the <see cref="IsInteractable"/> function.
	/// <param name="result"> The last Trace performed from the handler. </param>
	/// <param name="client"> The current simulating client </param>
	/// </summary>
	void Simulate( in TraceResult result, IClient client );
}
