using System;
using System.Collections.Generic;
using Sandbox;
using Woosh.Common;
using Woosh.Espionage.Data;

namespace Woosh.Espionage;

public static class EntityUtility
{
	// Cooler Display Info
	
	[SkipHotload] private readonly static Dictionary<(Type Type, int Hash), DisplayInfo> m_TypeInfo;

	static EntityUtility()
	{
		m_TypeInfo = new Dictionary<(Type, int), DisplayInfo>();
	}

	private static DisplayInfo? GetOrCreateInfoFromModel( Model model )
	{
		if ( model == null )
			return null;

		var key = (typeof(ModelEntity), model.GetHashCode());
		if ( m_TypeInfo.TryGetValue( key, out var info ) )
			return info;

		var maybe = model?.GetData<ModelDisplayInfo>()?.Info;

		if ( maybe != null )
			m_TypeInfo.Add( key, maybe.Value );

		return maybe;
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
		var info = (entity as IHave<DisplayInfo>)?.Item;
		if ( entity is ModelEntity model )
		{
			info ??= GetOrCreateInfoFromModel( model.Model );
		}
		return info ?? GetOrCreateInfoFromType( entity.GetType() );
	}
}
