namespace Woosh.Espionage;

public interface IPreMutate<T> where T : struct
{
	void OnPreMutate( ref T setup );
}

public interface IPostMutate<T> where T : struct
{
	void OnPostMutate( ref T setup );
}
