using Woosh.Common;

namespace Woosh.Espionage;

public readonly record struct CreateViewModel( CompositedViewModel ViewModel ) : IEventData;
