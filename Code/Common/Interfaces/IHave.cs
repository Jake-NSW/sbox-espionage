namespace Woosh.Espionage;

/// <summary>
/// An interface that is meant to represent a classes capability of having a particular piece of data. This allows us to not couple different
/// classes together while trying to get one piece of particular data out of it.
/// <typeparam name="T"> The type of data we want. </typeparam>
/// </summary>
public interface IHave<out T>
{
	/// <summary>
	/// The data that we want to provide to who ever is asking for it.
	/// </summary>
	T Item { get; }
}
