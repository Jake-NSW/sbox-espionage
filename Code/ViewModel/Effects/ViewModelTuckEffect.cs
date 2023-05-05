using System;
using Sandbox;

namespace Woosh.Espionage;

public enum TuckType
{
	Push,
	Rotate,
	Hug,
}

public sealed class ViewModelTuckEffect : IViewModelEffect
{
	public float Damping { get; set; } = 14;
	public TuckType Variant { get; set; }

	// Logic

	private float m_Offset;

	public bool Update( ref ViewModelSetup setup )
	{
		const string name = "muzzle";

		// Get the muzzle attachment
		var attachment = setup.Entity.GetAttachment( name );
		if ( !attachment.HasValue )
			return false;

		var muzzle = attachment.Value;
		var distance = muzzle.Position.Distance( setup.Entity.Position );

		var start = muzzle.Position - (muzzle.Rotation.Forward * distance);
		var end = start + (muzzle.Rotation.Forward * (distance * 2));
		var info = Trace.Ray( start, end )
			.Ignore( setup.Entity )
			.Ignore( setup.Owner )
			.Size( 2 )
			.Run();

		m_Offset = m_Offset.LerpTo( info.Hit && info.Distance < distance ? info.Distance - distance : 0, Damping * Time.Delta );
		var normal = MathF.Abs( m_Offset / distance );

		switch ( Variant )
		{
			case TuckType.Push :
				setup.Position += setup.Rotation * new Vector3( m_Offset, (normal * 3), (-normal * 8) * (1 - setup.Aim) );
				setup.Rotation *= Rotation.FromAxis( Vector3.Forward, -(normal * 65) );
				break;
			case TuckType.Rotate :
				setup.Position += setup.Rotation * new Vector3( -m_Offset / 6, 0, 0 );
				setup.Rotation *= Rotation.FromAxis( Vector3.Left, -normal * 90 );
				setup.Position += setup.Rotation * new Vector3( m_Offset / 1, 0, 0 );
				break;
			case TuckType.Hug :
				setup.Rotation *= Rotation.FromAxis( Vector3.Up, normal * 90 );
				setup.Position += setup.Rotation * new Vector3( m_Offset, 0, 0 );
				break;
			default :
				throw new ArgumentOutOfRangeException();
		}

		return false;
	}

	public void Register( IDispatchRegistryTable table ) { }
	public void Unregister( IDispatchRegistryTable table ) { }
}
