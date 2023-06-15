using System;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct PlayClientEffects<T>( T Effects ) : ISignal where T : Enum;
