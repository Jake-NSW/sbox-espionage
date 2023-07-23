namespace Woosh.Espionage;

/// <summary>
/// IMutate is an interface that is meant to represent a classes capability of iterating over a struct and mutating it. This is mutation
/// that is usually done within a simulation snapshot or every frame. 
/// <typeparam name="T"> The type of data we are going to iterate over </typeparam>
/// </summary>
public interface IMutate<T> where T : struct
{
	/// <summary>
	/// A callback provided for mutating the data in a particular way. 
	/// </summary>
	void OnMutate( ref T setup );
}
