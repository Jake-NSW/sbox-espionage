using System;
using Sandbox;

namespace Woosh.Espionage;

public struct SoundBank<T> where T : Enum
{
	public SoundBank() { }

	public void Assign( T index, string sound ) { }

	public Sound Play( T index )
	{
		return default;
	}
}
