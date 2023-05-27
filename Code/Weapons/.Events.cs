using System;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct PlayClientEffects<T>( T Effects ) : ISignal where T : Enum;

public readonly record struct FirearmSetupApplied( FirearmSetup setup ) : ISignal;
public readonly record struct FirearmRebuildRequest() : ISignal;

public readonly record struct WeaponFired( Vector3 Recoil, Vector3 Kickback ) : ISignal;

public readonly record struct ReloadStarted() : ISignal;
