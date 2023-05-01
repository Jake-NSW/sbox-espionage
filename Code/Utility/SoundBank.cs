using System;
using System.Collections.Generic;
using Sandbox;

namespace Woosh.Espionage;

public static class EnumValues<T> where T : Enum
{
	private readonly static T[] s_Values;

	public static int Length => s_Values.Length;

	public static int IndexOf( T input )
	{
		var compare = EqualityComparer<T>.Default;

		for ( var i = 0; i < s_Values.Length; i++ )
		{
			if ( compare.Equals( s_Values[i], input ) )
				return i + 1;
		}

		throw new InvalidOperationException();
	}

	static EnumValues()
	{
		s_Values = (T[])typeof(T).GetEnumValues();
	}
}

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
