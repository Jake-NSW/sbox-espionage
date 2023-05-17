namespace Woosh.Common;

public interface IHave<out T>
{
	T Item { get; }
}
