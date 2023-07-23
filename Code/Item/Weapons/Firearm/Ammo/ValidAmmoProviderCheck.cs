using System;
using System.Collections.Generic;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly struct ValidAmmoProviderCheck : ISignal
{
	private readonly HashSet<int> m_Hash;

	public ValidAmmoProviderCheck()
	{
		m_Hash = new HashSet<int>();
	}

	public void AddType<T>() where T : IFirearmAmmoProvider
	{
		m_Hash.Add( typeof(T).GetHashCode() );
	}

	public bool CheckType<T>() where T : IFirearmAmmoProvider
	{
		return CheckType( typeof(T) );
	}

	public bool CheckType( Type type )
	{
		return m_Hash.Contains( type.GetHashCode() );
	}
}
