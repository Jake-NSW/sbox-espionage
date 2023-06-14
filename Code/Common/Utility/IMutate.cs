namespace Woosh.Espionage;

public interface IMutate<T> where T : struct
{
	void OnPostSetup( ref T setup );
}
