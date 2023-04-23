namespace Woosh.Espionage;

public delegate void Callback<T>( in T evt ) where T : struct;
