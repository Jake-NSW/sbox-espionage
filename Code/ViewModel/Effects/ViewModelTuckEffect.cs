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

	private AnimatedEntity m_Model;

	public void Register( AnimatedEntity model, IDispatchRegistryTable table )
	{
		m_Model = model;
	}

	// Logic

	private float m_Offset;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		const string name = "muzzle";

		// Get the muzzle attachment
		var attachment = m_Model.GetAttachment( name );
		if ( !attachment.HasValue )
			return;

		var muzzle = attachment.Value;
		var distance = muzzle.Position.Distance( setup.Position );

		var start = muzzle.Position - (muzzle.Rotation.Forward * distance);
		var end = start + (muzzle.Rotation.Forward * (distance * 2));
		var info = Trace.Ray( start, end )
			.Ignore( Game.LocalPawn )
			.Size( 2 )
			.Run();

		m_Offset = m_Offset.LerpTo( info.Hit && info.Distance < distance ? info.Distance - distance : 0, Damping * Time.Delta );
		var normal = MathF.Abs( m_Offset / distance );

		var relativeRot = setup.Rotation * setup.Hands.Angles;

		switch ( Variant )
		{
			case TuckType.Push :
				setup.Hands.Offset += relativeRot * new Vector3( m_Offset, (normal * 3), (-normal * 8) * (1 - setup.Hands.Aim) );
				setup.Hands.Angles *= Rotation.FromAxis( Vector3.Forward, -(normal * 65) );
				break;
			case TuckType.Rotate :
				setup.Hands.Offset += relativeRot * new Vector3( -m_Offset / 6, 0, 0 );
				setup.Hands.Angles *= Rotation.FromAxis( Vector3.Left, -normal * 90 );
				setup.Hands.Offset += (setup.Rotation * setup.Hands.Angles) * new Vector3( m_Offset / 1, 0, 0 );
				break;
			case TuckType.Hug :
				setup.Hands.Angles *= Rotation.FromAxis( Vector3.Up, normal * 90 );
				setup.Hands.Offset += (setup.Rotation * setup.Hands.Angles) * new Vector3( m_Offset, 0, 0 );
				break;
			default :
				throw new ArgumentOutOfRangeException();
		}
	}
}
