using System;
using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Common;

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
