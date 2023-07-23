using System;
using System.Runtime.CompilerServices;

namespace Woosh.Espionage;

public struct InputContext
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public bool Pressed( string input ) => Sandbox.Input.Pressed( input );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public bool Released( string input ) => Sandbox.Input.Released( input );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public bool Down( string input ) => Sandbox.Input.Down( input );

	public Angles ViewAngles;
	public Vector3 InputDirection;
	
	[Obsolete("Incredibly hacky way of tracking muzzle position for now. (sbox doesn't provide a nice way to do this without rpcs)")]
	public Ray Muzzle;
}
