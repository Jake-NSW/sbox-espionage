using System;

namespace Woosh.Common;

public interface IDispatchRegistryTable
{
	public void Register<T>( StructCallback<T> callback ) where T : struct, IEventData;
	public void Register<T>( Action callback ) where T : struct, IEventData;

	public void Unregister<T>( Action callback ) where T : struct, IEventData;
	public void Unregister<T>( StructCallback<T> callback ) where T : struct, IEventData;
}
