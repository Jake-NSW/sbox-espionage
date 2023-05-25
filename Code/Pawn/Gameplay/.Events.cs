using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct LeanDirectionChanged( int Direction ) : ISignal;
