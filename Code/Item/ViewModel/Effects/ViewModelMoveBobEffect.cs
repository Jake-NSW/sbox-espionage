﻿using System;
using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelMoveBobEffect : ObservableEntityComponent<ViewModel>, IViewModelEffect
{
	private float m_Scale;
	private float m_Delta;
	private float m_Speed;
	private Vector3 m_Bob;

	public void OnMutate( ref CameraSetup setup )
	{
		var rot = setup.Rotation.WithRoll( 0 );
		
		var speed = Entity.Owner.Velocity.WithZ( 0 ).Length.LerpInverse( 0, 160 );
		m_Speed = m_Speed.LerpTo( speed, 4 * Time.Delta );
		m_Scale = m_Scale.LerpTo( Game.LocalPawn.GroundEntity != null ? speed : 0, 10 * Time.Delta );
		m_Delta += Time.Delta * 15.0f * m_Scale;

		// Waves
		m_Bob.x = MathF.Sin( m_Delta * 0.7f ) * 0.6f;
		m_Bob.y = MathF.Cos( m_Delta * 0.7f ) * 0.4f;
		m_Bob.z = MathF.Cos( m_Delta * 1.3f ) * 0.8f;

		// Scale walk bob off property
		m_Bob *= m_Speed;

		setup.Hands.Offset += rot * new Vector3( 0, m_Bob.y * 1.2f, m_Bob.z * 0.8f );
		setup.Hands.Angles *= Rotation.From( m_Bob.z * 2, m_Bob.y * 4, m_Bob.x * 4 );
		
		const float cameraScale = 0f;

		setup.Rotation *= Rotation.Lerp( Rotation.Identity, Rotation.From( -m_Bob.z, -m_Bob.y * 2, -m_Bob.x * 3 ), cameraScale, false );
	}
}
