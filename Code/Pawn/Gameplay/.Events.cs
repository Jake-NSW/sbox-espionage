using Woosh.Common;

namespace Woosh.Espionage;

public readonly record struct LeanDirectionChanged( int Direction ) : IEventData;
