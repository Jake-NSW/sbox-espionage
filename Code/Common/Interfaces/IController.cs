namespace Woosh.Espionage;

/// <summary>
/// A Controller is a class that can be mutated via an input context. Controllers are always called after the input context has been updated.
/// Usually a controller is in the context of a pawn. In some cases though it can be in the context of directly manipulating something.
/// <typeparam name="T"> The type of data we would like to iterate over. </typeparam>
/// </summary>
public interface IController<T> where T : struct
{
	/// <summary>
	/// Called when this controller is actively being used.
	/// <param name="setup"> The data that will be iterated over </param>
	/// </summary>
	void Activate( ref T setup ) { }

	/// <summary>
	/// Called either every frame or every simulate snapshot depending on where this controller is being used.
	/// <param name="setup"> The data that is being iterated over </param>
	/// <param name="input"> The active input context that mutates the data in a particular way </param>
	/// </summary>
	void Update( ref T setup, in InputContext input );

	/// <summary>
	/// Called when this controller is no longer being used.
	/// </summary>
	void Disabled() { }
}
