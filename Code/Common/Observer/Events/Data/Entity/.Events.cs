using Sandbox;
using Woosh.Signals;

namespace Woosh.Common;

// Base Entity

public readonly record struct ComponentAdded( EntityComponent Component ) : ISignal;

public readonly record struct ComponentRemoved( EntityComponent Component ) : ISignal;

// Model Entity

public readonly record struct ModelChanged( Model Model ) : ISignal;
