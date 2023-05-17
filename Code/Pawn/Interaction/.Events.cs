using System.Collections.Generic;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly record struct InteractionTargetChanged( Entity Hovering, IReadOnlyList<IEntityInteraction> Interactions ) : IEventData;

