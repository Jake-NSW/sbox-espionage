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

		Run( new WeaponFired( new Vector3( -65, 10f, 10f ), new Vector3( -35, 10f, 10f ) ), Propagation.Both );

		// Play Effects
		if ( Game.IsServer )
		{
			var effects = WeaponClientEffects.Attack;

			if ( Setup.IsSilenced )
				effects |= WeaponClientEffects.Silenced;

			PlayClientEffects( effects );
		}

		// Owner, Shoot from View Model
		if ( Entity.Owner != null && Entity.IsLocalPawn )
		{
			Log.Info( "Shooting on Client" );

			if ( Entity.Effects?.GetAttachment( "muzzle" ) is { } muzzle )
			{
				CmdReceivedShootRequest( Entity.NetworkIdent, muzzle.Position, muzzle.Rotation.Forward );
			}

			return;
		}

		// No Owner, Shoot from World Model
		if ( Entity.Owner == null && Game.IsServer ) { }
	}

	[ClientRpc]
	private void PlayClientEffects( WeaponClientEffects effects )
	{
		Run( new PlayClientEffects<WeaponClientEffects>( effects ) );
	}

	private TimeSince m_SinceShoot;
	private Vector3 m_Forward;
	private Vector3 m_Position;

	private Vector3 m_LastPosition;

	private bool m_Running;

	[GameEvent.Tick]
	private void Tick()
	{
		if ( !m_Running )
			return;

		var helper = new ProjectMovementHelper( 300, 0.0009f );

		var position = helper.AtTime( m_SinceShoot, m_Position, m_Forward );
		var direction = (position - m_LastPosition).Normal;

		var ray = Trace.Ray( m_LastPosition, position )
			.UseHitboxes()
			.WithAnyTags( "solid", "player", "npc" )
			.Ignore( Entity.Owner )
			.Size( 1 )
			.Run();

		if ( ray.Hit )
		{
			Log.Info( "HIT!" );
			ray.Surface.DoBulletImpact( ray );

			var info = DamageInfo.FromBullet( ray.EndPosition, direction * 300, 100 );
			ray.Entity.TakeDamage( info );
			m_Running = false;
		}

		m_LastPosition = position;
	}

	[ConCmd.Server]
	private static void CmdReceivedShootRequest( int indent, Vector3 pos, Vector3 forward )
	{
		// DebugOverlay.Sphere( pos, 1, Color.Red, duration: 5 );
		// DebugOverlay.Line( pos, pos + forward * 8, Color.Green, duration: 5 );

		var helper = new ProjectMovementHelper( 300, 0.0009f );
		// DebugOverlay.Projectile( helper, pos, forward, 5 );

		var firearm = Sandbox.Entity.FindByIndex<Firearm>( indent );
		var comp = firearm.Components.Get<FirearmShootSimulatedEntityState>();
		comp.m_SinceShoot = 0;
		comp.m_Forward = forward;
		comp.m_Position = pos;
		comp.m_LastPosition = pos;
		comp.m_Running = true;

		Log.Info( "Shooting on Server" );
	}
}
