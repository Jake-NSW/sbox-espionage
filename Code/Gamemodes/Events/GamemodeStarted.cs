using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct GamemodeStarted( IGamemode Gamemode ) : ISignal;
