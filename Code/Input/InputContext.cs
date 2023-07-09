using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

public struct InputContext
{
	public Angles AnalogLook
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Input.AnalogLook;
	}

	public Angles ViewAngles;
	public Vector3 InputDirection;

	public Ray Muzzle;
}
