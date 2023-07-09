using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public static class EntityUtility
{
#nullable enable
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T? Has<T>( this Entity ent )
	{
		if ( ent is IHave<T> have )
		{
			return have.Item;
		}

		return default;
	}
#nullable restore

	// Cooler Display Info

	[SkipHotload] private readonly static Dictionary<(Type Type, int Hash), DisplayInfo> m_TypeInfo;

	static EntityUtility()
	{
		m_TypeInfo = new Dictionary<(Type, int), DisplayInfo>();
	}
	
	private static DisplayInfo GetOrCreateInfoFromType( Type type )
	{
		if ( m_TypeInfo.TryGetValue( (type, 0), out var info ) )
			return info;

		info = DisplayInfo.ForType( type );
		m_TypeInfo.Add( (type, 0), info );
		return info;
	}

	public static DisplayInfo Info( this Entity entity )
	{
		return entity == null ? new DisplayInfo() { Name = "Unknown" } : GetOrCreateInfoFromType( entity.GetType() );
	}
}
