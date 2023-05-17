namespace Woosh.Common;

public interface IActive<out T>
{
	T Active { get; }
}
