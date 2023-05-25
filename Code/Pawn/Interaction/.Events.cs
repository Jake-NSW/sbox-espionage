using System.Collections.Generic;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct InteractionTargetChanged( Entity Hovering, IReadOnlyList<IEntityInteraction> Interactions ) : ISignal;

