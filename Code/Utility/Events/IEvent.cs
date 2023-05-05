namespace Woosh.Espionage;

public interface ICallback<T> where T : struct, IEvent
{
	void Execute( in T evt );
}

public interface IEvent { }
