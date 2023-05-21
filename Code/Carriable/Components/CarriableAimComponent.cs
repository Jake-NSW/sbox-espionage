using System;
using Sandbox;
using Sandbox.Utility;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed partial class CarriableAimComponent : ObservableEntityComponent<ICarriable>, IMutateCameraSetup
{
	public Easing.Function Easing => Sandbox.Utility.Easing.QuadraticInOut;

	public bool IsAiming => n_IsAiming;
	[Net, Predicted, Local] private bool n_IsAiming { get; set; }
	private float AimSpeed => 2.5f;

	public void Simulate( IClient cl )
	{
		if ( CanAim( false ) )
		{
			ToggleAim( false );
		}
	}


	private void ToggleAim( bool toggle )
	{
		if ( toggle )
		{
			if ( Input.Pressed( "aim" ) )
				n_IsAiming = !n_IsAiming;
		}
		else
		{
			n_IsAiming = Input.Down( "aim" );
		}
	}

	private bool CanAim( bool toggleAim = true )
	{
		var owner = Entity.Owner as Entity;

		// If toggle aim and is not grounded, aim out
		if ( owner!.GroundEntity == null && toggleAim )
			return true;

		// If hold down aim, just return true
		if ( !toggleAim )
			return true;

		return Input.Pressed( "aim" );
	}

	private float m_Delta;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		m_Delta += IsAiming ? Time.Delta * AimSpeed : -Time.Delta * AimSpeed;
		m_Delta = m_Delta.Min( 1 ).Max( 0 );

		var aim = Easing( m_Delta );
		setup.Hands.Aim = aim;
		setup.FieldOfView -= aim * 10;
		
		// Workout Angles
		
		var arch = aim * (1 - aim);
		var startArch = (aim * aim) * (1 - aim);
		
		var endArch = aim * (1 - aim);
		endArch *= endArch;
		
		setup.Hands.Angles *= Rotation.FromRoll( arch * -45 ) * Rotation.FromPitch(startArch  * 45);
		setup.Hands.Offset += setup.Rotation.Down * endArch * 8; 
	}
}
