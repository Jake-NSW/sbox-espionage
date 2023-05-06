namespace Woosh.Espionage;

public interface ITemplate<in T> where T : class
{
	void Generate( T value );
}
