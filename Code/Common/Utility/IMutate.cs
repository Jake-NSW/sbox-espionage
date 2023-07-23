namespace Woosh.Espionage;

public interface IMutate<T> where T : struct
{
	void OnMutate( ref T setup );
}
