using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class FirearmEffectsHandler : ObservableEntityComponent
{
	[Listen]
	private void OnClientEffects( Event<PlayClientEffects<FirearmClientEffects>> evt )
	{
		if ( Entity.IsFirstPersonMode )
			return;

		// Get Muzzle
		// Particles.Create( "particles/weapons/muzzle_flash/muzzleflash_flash.vpcf", Entity, "muzzle" );

		// This should come out when the slide is racked? Maybe good idea to make an animgraph event for it
		var particle = Particles.Create( "particles/weapons/ejection/pistol_ejectbrass.vpcf", Entity, "eject_port" );
	}
}
