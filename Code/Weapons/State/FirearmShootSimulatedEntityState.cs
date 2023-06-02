using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class FirearmShootSimulatedEntityState : ObservableEntityComponent<Firearm>, ISimulatedEntityState<Firearm>, ISingletonComponent
{
	public FirearmSetup Setup => Entity.Setup;

	public bool TryEnter()
	{
		return IsFireable( true );
	}

	public bool Simulate( IClient cl )
	{
		if ( IsFireable() )
			Shoot();

		return !Setup.IsAutomatic || !Input.Down( "shoot" );
	}

	public void OnStart() { }
	public void OnFinish() { }

	// Logic

	protected override void OnActivate()
	{
		base.OnActivate();
		n_SinceLastShot = 0;
	}

	[Net, Predicted, Local] private TimeSince n_SinceLastShot { get; set; }

	public bool IsFireable( bool checkInput = false )
	{
		// Check if any state is running
		if ( checkInput )
		{
			// Check for input
			if ( Setup.IsAutomatic ? !Input.Down( "shoot" ) : !Input.Pressed( "shoot" ) )
				return false;
		}

		return n_SinceLastShot >= 60 / Setup.RateOfFire;
	}

	public void Shoot()
	{
		n_SinceLastShot = 0;

		if ( !Prediction.FirstTime )
			return;

		Events.Run( new WeaponFired( new Vector3( -5, 0f, 0 ) * 35, new Vector3( -1, 0f, 0f ) * 0 ) );

		// Play Effects
		if ( Game.IsServer )
		{
			var effects = WeaponClientEffects.Attack;

			if ( Setup.IsSilenced )
				effects |= WeaponClientEffects.Silenced;

			PlayClientEffects( effects );
		}

		// Owner, Shoot from View Model
		if ( Entity.IsLocalPawn )
		{
			var muzzle = Entity.Effects?.Target?.GetAttachment( "muzzle" ) ?? Entity.Owner.Transform;
			CmdReceivedShootRequest( NetworkIdent, muzzle.Position, muzzle.Rotation.Forward );
			return;
		}

		// No Owner, Shoot from World Model
		if ( Entity.Owner == null && Game.IsServer ) { }
	}

	[ClientRpc]
	private void PlayClientEffects( WeaponClientEffects effects )
	{
		// Sounds.Play( effects, Owner?.AimRay.Position ?? Position );
		Events.Run( new PlayClientEffects<WeaponClientEffects>( effects ) );
	}

	[ConCmd.Server]
	private static void CmdReceivedShootRequest( int indent, Vector3 pos, Vector3 forward ) { }
}
