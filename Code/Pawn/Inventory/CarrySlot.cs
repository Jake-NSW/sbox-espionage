using System;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

public enum CarrySlot
{
	Front, Back, Holster, Grenade, Utility
}

public static class CarrySlotUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static string ToName( this CarrySlot slot )
	{
		return slot switch
		{
			CarrySlot.Front => "Front",
			CarrySlot.Back => "Back",
			CarrySlot.Holster => "Holster",
			CarrySlot.Grenade => "Grenade",
			CarrySlot.Utility => "Utility",
			_ => throw new ArgumentOutOfRangeException( nameof(slot), slot, null )
		};
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Texture ToIcon( this CarrySlot slot )
	{
		return slot switch
		{
			CarrySlot.Front => null,
			CarrySlot.Back => null,
			CarrySlot.Holster => null,
			CarrySlot.Grenade => null,
			CarrySlot.Utility => null,
			_ => throw new ArgumentOutOfRangeException( nameof(slot), slot, null )
		};
	}
}
