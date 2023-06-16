using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class WeaponClientEffectsHandler : ObservableEntityComponent
{
	[Listen]
	private void OnClientEffects( Event<PlayClientEffects<WeaponClientEffects>> evt )
	{
		if ( Entity.IsFirstPersonMode )
			return;
		
		// Get Muzzle
		// Particles.Create( "particles/weapons/muzzle_flash/muzzleflash_flash.vpcf", Entity, "muzzle" );
		
		// This should come out when the slide is racked? Maybe good idea to make an animgraph event for it
		Particles.Create( "particles/weapons/ejection/pistol_ejectbrass.vpcf", Entity, "eject_port" );
	}
}
