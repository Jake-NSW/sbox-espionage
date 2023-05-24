using System;
using Sandbox;
using Sandbox.Utility;
using Woosh.Common;

namespace Woosh.Espionage;

public enum TuckType
{
	Push,
	Rotate,
	Hug,
}

public sealed class ViewModelTuckEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float Damping { get; set; } = 14;
	public TuckType AimVariant { get; init; }
	public TuckType HipVariant { get; init; }

	// Logic

	private float m_Offset;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		const string name = "muzzle";

		// Get the muzzle attachment
		var attachment = Entity.GetAttachment( name );
		if ( !attachment.HasValue )
			return;

		var muzzle = attachment.Value;
		var distance = muzzle.Position.Distance( Entity.Position );

		var start = muzzle.Position - (muzzle.Rotation.Forward * distance);
		var end = start + (muzzle.Rotation.Forward * (distance * 2));
		var info = Trace.Ray( start, end )
			.Ignore( Game.LocalPawn )
			.Size( 1 )
			.Run();

		m_Offset = m_Offset.LerpTo( info.Hit && info.Distance < distance ? info.Distance - distance : 0, Damping * Time.Delta );
		var normal = MathF.Abs( m_Offset / distance );

		CameraSetup hip = new CameraSetup( setup );
		Calculate( HipVariant, normal, ref hip );
		CameraSetup aim = new CameraSetup( setup );
		Calculate( AimVariant, normal, ref aim );

		var last = setup;
		setup = CameraSetup.Lerp( hip, aim, Easing.QuadraticInOut( setup.Hands.Aim ) );
		setup.Viewer = last.Viewer;
	}

	private void Calculate( TuckType type, float normal, ref CameraSetup setup )
	{
		var relativeRot = setup.Rotation * setup.Hands.Angles;

		switch ( type )
		{
			case TuckType.Push :
				setup.Hands.Offset += relativeRot * new Vector3( m_Offset, (normal * 3) * (1 - setup.Hands.Aim), (-normal * 8) );
				setup.Hands.Angles *= Rotation.FromAxis( Vector3.Forward, -(normal * 65) * (1 - setup.Hands.Aim) );
				break;
			case TuckType.Rotate :
				setup.Hands.Offset += relativeRot * new Vector3( -m_Offset / 3.4f, 0, 0 );
				setup.Hands.Angles *= Rotation.FromAxis( Vector3.Left, -normal * 90 );
				setup.Hands.Offset += (setup.Rotation * setup.Hands.Angles) * new Vector3( m_Offset / 1.1f, 0, 0 );
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
