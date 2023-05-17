using Sandbox;

namespace Woosh.Common;

// Base Entity

public readonly record struct ComponentAdded( EntityComponent Component ) : IEventData;

public readonly record struct ComponentRemoved( EntityComponent Component ) : IEventData;

// Model Entity

public readonly record struct ModelChanged( Model Model ) : IEventData;
