using System.Collections.Generic;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ProjectileSimulator : ObservableEntityComponent<Project>, ISimulated, INetworkSerializer
{
	public ProjectileSimulator()
	{
		m_InMotion = new Dictionary<ProjectileDetails, Vector3>( 16 );
	}

	private readonly Dictionary<ProjectileDetails, Vector3> m_InMotion;

	public void Add( ProjectileDetails details )
	{
		m_InMotion.Add( details, details.Start - details.Forward * 4 );
		WriteNetworkData();
	}

	public void Remove( ProjectileDetails details )
	{
		m_InMotion.Remove( details );
		// WriteNetworkData();
	}

	public void Clear( ProjectileDetails details )
	{
		m_InMotion.Clear();
		WriteNetworkData();
	}

	public void Simulate( IClient client )
	{
		foreach ( var (details, last) in m_InMotion )
		{
			var helper = new ProjectileMovementHelper( details );
			var pos = helper.AtTime( details.Since, details.Start, details.Forward );
			var direction = (pos - last).Normal;

			if ( details.Since > 3 )
			{
				Log.Info( "Faded Away!" );
				Remove( details );
				return;
			}

			var ray = Trace.Ray( last, pos ).UseHitboxes()
				.WithAnyTags( "solid", "player", "npc" )
				.Ignore( Entity.Owner )
				.Size( 1 )
				.Run();

			if ( ray.Hit )
			{
				Log.Info("Hit!");
				
				if ( Game.IsClient )
				{
					ray.Surface.DoBulletImpact( ray );
				}

				if ( Game.IsServer )
				{
					var info = DamageInfo.FromBullet( ray.EndPosition, direction * details.Force, 100 )
						.WithAttacker( Sandbox.Entity.FindByIndex( details.Attacker ) )
						.WithWeapon( Sandbox.Entity.FindByIndex( details.Weapon ) );

					ray.Entity.TakeDamage( info );
				}

				Remove( details );
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
			m_InMotion.Add( details, details.Start );
		}
	}

	public void Write( NetWrite write )
	{
		write.Write( m_InMotion.Count );
		foreach ( var details in m_InMotion )
		{
			write.Write( details );
		}
	}
}
