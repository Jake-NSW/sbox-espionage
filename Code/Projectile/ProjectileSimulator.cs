using System.Collections.Generic;
using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ProjectileSimulator : ObservableEntityComponent<App>, INetworkSerializer
{
	public ProjectileSimulator()
	{
		m_InMotion = new List<ProjectileSnapshot>( 32 );
	}

	private readonly List<ProjectileSnapshot> m_InMotion;

	public void Add( ProjectileSnapshot snapshot )
	{
		m_InMotion.Add( snapshot );
	}

	public void Remove( int index )
	{
		m_InMotion.RemoveAt( index );
	}

	public void Clear( ProjectileSnapshot snapshot )
	{
		m_InMotion.Clear();
		WriteNetworkData();
	}

	[ConVar.Server( "esp_projectile_debug" )]
	private static bool EnableProjectileDebug { get; set; }

	[Listen]
	private void OnSimulate( Event<SimulateSnapshot> evt )
	{
		if ( !Prediction.FirstTime )
			return;

		for ( var i = 0; i < m_InMotion.Count; i++ )
		{
			var details = m_InMotion[i];

			if ( details.Since > 3 )
			{
				Remove( i );
				continue;
			}

			var helper = new ProjectileMovementHelper( details );

			var pos = helper.AtTime( details.Since + Time.Delta, details.Start, details.Forward );
			var last = helper.AtTime( details.Since, details.Start, details.Forward );
			var direction = (pos - last).Normal;

			var attacker = Sandbox.Entity.FindByIndex( details.Attacker );

			// Play Trace
			var ray = Trace.Ray( last, pos )
				.UseHitboxes()
				.WithAnyTags( "solid", "player", "npc", "glass", "prop" )
				.Ignore( attacker )
				.Size( 1 )
				.Run();

			const float debugTime = 5;
			if ( EnableProjectileDebug )
			{
				DebugOverlay.Line( last, pos, Game.Color.WithAlpha( 0.7f ), debugTime );
			}

			if ( !ray.Hit )
				continue;

			if ( EnableProjectileDebug )
			{
				var meters = ray.EndPosition.Distance( details.Start ) / 39.37f;
				DebugOverlay.Text( $"{meters}m", ray.HitPosition, debugTime, 99999999999 );
			}

			ray.Surface.DoBulletImpact( ray );
			Remove( i );

			if ( Game.IsServer )
			{
				var weapon = Sandbox.Entity.FindByIndex( details.Weapon );
				var info = DamageInfo.FromBullet( ray.EndPosition, direction * details.Force, 100 )
					.WithAttacker( attacker )
					.WithWeapon( weapon )
					.WithHitBody( ray.Body )
					.WithBone( ray.Bone );

				ray.Entity.TakeDamage( info );
			}
		}
	}

	public void Read( ref NetRead read )
	{
		m_InMotion.Clear();

		var length = read.Read<int>();
		m_InMotion.EnsureCapacity( length );

		for ( var i = 0; i < length; i++ )
		{
			var details = read.Read<ProjectileSnapshot>();
			m_InMotion.Add( details );
		}
	}

	public void Write( NetWrite write )
	{
		write.Write( m_InMotion.Count );
		foreach ( var projectile in m_InMotion )
		{
			write.Write( projectile );
		}
	}
}
