using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct FirearmSetupApplied( FirearmSetup setup ) : ISignal;
