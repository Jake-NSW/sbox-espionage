using System.Runtime.CompilerServices;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class ShootFirearmState : ObservableEntityComponent<Firearm>, IEntityState<Firearm>, ISingletonComponent, IMutate<InputContext>
{
	public FirearmSetup Setup => Entity.Setup;

	public bool TryEnter()
	{
		return IsFireable( true );
	}

	public bool Simulate( IClient cl )
	{
		if ( IsFireable() && Prediction.FirstTime )
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

		Run( ApplyRecoilFromSetup( Setup ), Propagation.Both );
		var muzzle = (Entity.Owner as Pawn)?.Muzzle ?? (Entity.GetAttachment( "muzzle" ) ?? Entity.Transform).ToRay();

		var effects = FirearmClientEffects.Attack;

		if ( Setup.IsSilenced )
			effects |= FirearmClientEffects.Silenced;

		// Apply impulse, as we are not in inventory
		if ( Game.IsServer && Entity.Owner == null )
			Entity.ApplyAbsoluteImpulse( -muzzle.Forward * 500 + Vector3.Random * 25 );

		PlayClientEffects( effects );
		Game.SetRandomSeed( Time.Tick );

		// Owner, Shoot from View Model
		App.Get<ProjectileSimulator>().Add(
			new ProjectileDetails
			{
				Force = Setup.Force,
				Mass = 0.0009f,
				Start = muzzle.Position,
				Forward = (Rotation.LookAt( muzzle.Forward ) * Rotation.FromYaw( Setup.Spread.Range() ) * Rotation.FromPitch( Setup.Spread.Range() )).Forward,
				Attacker = Entity.Owner.NetworkIdent,
				Weapon = Entity.NetworkIdent,
				Since = 0
			}
		);
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static FirearmFired ApplyRecoilFromSetup( in FirearmSetup setup )
	{
		// Do Later...
		return new FirearmFired( new Vector3( -65, 10f, 10f ), new Vector3( -35, 10f, 10f ) );
	}

	[ClientRpc]
	private void PlayClientEffects( FirearmClientEffects effects )
	{
		Run( new PlayClientEffects<FirearmClientEffects>( effects ) );
	}

	public void OnMutate( ref InputContext setup )
	{
		if ( Entity.Effects?.GetAttachment( "muzzle" ) is { } muzzle )
			setup.Muzzle = new Ray( muzzle.Position, muzzle.Rotation.Forward );
		else
			setup.Muzzle = default;
	}

	[Listen]
	private void OnTakeDamage( Event<EntityTakenDamage> evt )
	{
		if ( Game.IsClient )
			return;

		var info = evt.Signal.LastDamageInfo;
		if ( !info.HasTag( "blunt" ) )
			return;

		if ( IsFireable() )
			Shoot();
	}
}
