using System;
using System.Collections.Generic;
using Sandbox;

namespace Woosh.Common;

public readonly struct SoundBank<T> where T : Enum
{
	// Add support for flags!! - was the whole point of it!!!

	private readonly string[] m_Sounds;

	public SoundBank()
	{
		m_Sounds = new string[EnumValues<T>.Length];
	}

	public SoundBank( IReadOnlyDictionary<T, string> values ) : this()
	{
		foreach ( var (key, value) in values )
		{
			Assign( key, value );
		}
	}

	public void Assign( T index, string sound )
	{
		m_Sounds[EnumValues<T>.IndexOf( index )] = sound;
	}

	public Sound Play( T index, Vector3 pos )
	{
		var value = m_Sounds[EnumValues<T>.IndexOf( index )];
		return value == null ? default : Sound.FromWorld( value, pos );
	}
}
