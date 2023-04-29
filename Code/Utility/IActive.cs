namespace Woosh.Espionage;

public interface IHave<out T>
{
	T Item { get; }
}

public interface IActive<out T>
{
	T Active { get; }
}
