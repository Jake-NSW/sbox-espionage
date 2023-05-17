namespace Woosh.Common;

public interface IComposite<T> where T : struct
{
	void OnCompositeSetup( ref T item );
}
