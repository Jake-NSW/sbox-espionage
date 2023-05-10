using Sandbox;
using Woosh.Common;
using Woosh.Espionage.Data;

namespace Woosh.Espionage;

public static class ObjectUtility
{
	public static DisplayInfo Info( this Entity entity )
	{
		var info = (entity as IHave<DisplayInfo>)?.Item;

		if ( entity is ModelEntity model )
		{
			info ??= model.Model?.GetData<ModelDisplayInfo>()?.Info;
		}

		return info ?? DisplayInfo.ForType( entity.GetType() );
	}
}
