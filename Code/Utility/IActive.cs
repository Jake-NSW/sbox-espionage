namespace Woosh.Espionage;

public interface IActive<out T> 
{
	T Active { get; }
}
