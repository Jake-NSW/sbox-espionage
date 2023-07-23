using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct CreatedViewModel( ViewModel ViewModel, Entity Target ) : ISignal;
