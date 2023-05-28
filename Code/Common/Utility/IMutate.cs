namespace Woosh.Espionage;

public delegate void Mutate<T>( ref T setup );

public interface IMutate<T> where T : struct
{
	void OnPostSetup( ref T setup );
}
