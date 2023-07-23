using System;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

public interface ISlotted : IEntity
{
	int Slot { get; }
}

public interface ISlotted<out T> : ISlotted where T : Enum
{
	int ISlotted.Slot
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get => EnumUtility<T>.IndexOf( Slot );
	}

	new T Slot { get; }
}
