using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct GamemodeFinished( IGamemode Gamemode ) : ISignal;
