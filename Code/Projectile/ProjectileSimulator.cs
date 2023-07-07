using System.Collections.Generic;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ProjectileSimulator : ObservableEntityComponent<App>, ISimulated, INetworkSerializer
{
	public ProjectileSimulator()
	{
		m_InMotion = new List<ProjectileDetails>( 32 );
	}

	private readonly List<ProjectileDetails> m_InMotion;

	public void Add( ProjectileDetails details )
	{
		m_InMotion.Add( details );
		// WriteNetworkData();
	}

	public void Remove( int index )
	{
		m_InMotion.RemoveAt( index );
	}

	public void Clear( ProjectileDetails details )
	{
		m_InMotion.Clear();
		WriteNetworkData();
	}

	public void Simulate( IClient client )
	{
		if ( !Prediction.FirstTime )
			return;

		for ( var i = 0; i < m_InMotion.Count; i++ )
		{
			var details = m_InMotion[i];
			var helper = new ProjectileMovementHelper( details );
			var pos = helper.AtTime( details.Since, details.Start, details.Forward );
			var last = helper.AtTime( details.Since - Time.Delta, details.Start, details.Forward );
			var direction = (pos - last).Normal;

			if ( details.Since > 3 )
			{
				Log.Info( $"Faded Away! - {details.Since}" );
				Remove( i );
				return;
			}

			var attacker = Sandbox.Entity.FindByIndex( details.Attacker );
			var weapon = Sandbox.Entity.FindByIndex( details.Weapon );

			// Play Trace
			var ray = Trace.Ray( last, pos )
				.UseHitboxes()
				.WithAnyTags( "solid", "player", "npc", "glass", "prop" )
				.Ignore( attacker )
				.Ignore( weapon )
				.Size( 1 )
				.Run();

			if ( !ray.Hit )
			{
				continue;
			}

			ray.Surface.DoBulletImpact( ray );
			Remove( i );

			if ( Game.IsServer )
			{
				var info = DamageInfo.FromBullet( ray.EndPosition, direction * details.Force * 4, 100 )
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
			var details = read.Read<ProjectileDetails>();
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
