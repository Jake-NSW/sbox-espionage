using System.Collections.Generic;

namespace Woosh.Espionage;

public static class DictionaryUtility
{
	public static void AddOrReplace<TKey, TValue>( this IDictionary<TKey, TValue> dic, TKey key, TValue value )
	{
		if ( !dic.TryAdd( key, value ) )
		{
			dic[key] = value;
		}
	}
}
