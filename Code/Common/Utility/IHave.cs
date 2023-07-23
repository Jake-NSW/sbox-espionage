namespace Woosh.Espionage;

public interface IHave<out T>
{
	T Item { get; }
}
