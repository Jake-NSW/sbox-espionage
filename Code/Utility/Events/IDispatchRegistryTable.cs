using System;

namespace Woosh.Espionage;

public interface IDispatchRegistryTable
{
	public void Register<T>( Callback<T> callback ) where T : struct, IEvent;
	public void Register<T>( Action callback ) where T : struct, IEvent;

	public void Unregister<T>( Action callback ) where T : struct, IEvent;
	public void Unregister<T>( Callback<T> callback ) where T : struct, IEvent;
}
