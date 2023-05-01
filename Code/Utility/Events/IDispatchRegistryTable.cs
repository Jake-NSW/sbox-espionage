using System;

namespace Woosh.Espionage;

public interface IDispatchRegistryTable
{
	public void Register<T>( StructCallback<T> callback ) where T : struct, IEvent;
	public void Register<T>( Action callback ) where T : struct, IEvent;

	public void Unregister<T>( Action callback ) where T : struct, IEvent;
	public void Unregister<T>( StructCallback<T> callback ) where T : struct, IEvent;
}
