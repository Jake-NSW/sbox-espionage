using System;
using Sandbox;
using Sandbox.Utility;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public enum TuckType
{
	Push,
	Rotate,
	Hug,
}

public sealed class ViewModelTuckEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect, IPostMutate<InputContext>
{
	public float Damping { get; set; } = 14;
	public TuckType AimVariant { get; init; }
	public TuckType HipVariant { get; init; }
	public float MaxNormal { get; init; } = 0.4f;

	// Logic

	private float m_Offset;
	private float m_Normal;

	public void OnPostMutate( ref CameraSetup setup )
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

		var range = info.Hit && info.Distance < distance ? info.Distance - distance : 0;
		m_Offset = m_Offset.Damp( range, Damping, Time.Delta );
		m_Normal = MathF.Abs( m_Offset / distance );

		CameraSetup hip = new CameraSetup( setup );
		Calculate( HipVariant, m_Normal, ref hip );
		CameraSetup aim = new CameraSetup( setup );
		Calculate( AimVariant, m_Normal, ref aim );

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
				setup.Hands.Angles *= Rotation.FromAxis( Vector3.Forward, -(normal * 65) );
				break;
			case TuckType.Rotate :
				setup.Hands.Offset += relativeRot * new Vector3( -m_Offset / 3.4f, 0, 0 );
				setup.Hands.Angles *= Rotation.FromAxis( Vector3.Left, -normal * 90 );
				setup.Hands.Offset += (setup.Rotation * setup.Hands.Angles) * new Vector3( m_Offset / 1.1f, 0, 0 );
				break;
			case TuckType.Hug :
				setup.Hands.Offset += setup.Rotation * new Vector3( 6 * normal, 0, 0 );
				setup.Hands.Angles *= Rotation.FromAxis( Vector3.Up, normal * 140 );
				setup.Hands.Offset += (setup.Rotation.WithRoll( 0 ) * setup.Hands.Angles.WithRoll( 0 )) * new Vector3( m_Offset, 0, 0 );
				break;
			default :
				throw new ArgumentOutOfRangeException();
		}
	}

	public void OnPostMutate( ref InputContext setup )
	{
		// We can't shoot if we're to close to the wall
		if ( m_Normal > MaxNormal )
		{
			Input.SetAction( "shoot", false );
		}
	}
}
