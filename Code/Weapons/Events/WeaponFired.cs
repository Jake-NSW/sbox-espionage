using Woosh.Common;

namespace Woosh.Espionage;

public readonly record struct WeaponFired( Vector3 Recoil, Vector3 Kickback ) : IEventData;
