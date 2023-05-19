using System;
using System.Collections.Generic;
using Sandbox;

namespace Woosh.Common;

public readonly struct SoundBank<T> where T : Enum
{
	private readonly Dictionary<int, string> m_Bank;

	public SoundBank()
	{
		m_Bank = new Dictionary<int, string>();
	}

	public string this[ T index ]
	{
		get => m_Bank.TryGetValue( index.GetHashCode(), out var value ) ? value : null;
		init => Add(index, value);
	}

	public void Add( T index, string sound )
	{
		m_Bank.AddOrReplace( index.GetHashCode(), sound );
	}

	public Sound Play( T index, Vector3 pos )
	{
		var value = this[index];
		return value == null ? default : Sound.FromWorld( value, pos );
	}
}
