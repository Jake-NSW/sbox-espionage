using System;
using Woosh.Common;

namespace Woosh.Espionage;

public readonly record struct PlayClientEffects<T>( T Effects ) : IEventData where T : Enum;

public readonly record struct FirearmSetupApplied( FirearmSetup setup ) : IEventData;

public readonly record struct WeaponFired( Vector3 Recoil, Vector3 Kickback ) : IEventData;
