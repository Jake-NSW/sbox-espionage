using Sandbox;
using Sandbox.Utility;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class CarriableAimComponent : ObservableEntityComponent<ICarriable>, IMutate<CameraSetup>, ISimulated
{
	public bool IsAiming => n_IsAiming;
	[Net, Predicted, Local] private bool n_IsAiming { get; set; }
	private float AimSpeed => 2.70f;

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

	public void OnPostSetup( ref CameraSetup setup )
	{
		m_Delta += IsAiming ? Time.Delta * AimSpeed : -Time.Delta * AimSpeed;
		m_Delta = m_Delta.Min( 1 ).Max( 0 );

		var aim = Easing.QuadraticInOut( m_Delta );
		setup.Hands.Aim = aim;
		setup.FieldOfView -= Easing.ExpoOut( Easing.EaseIn( m_Delta ) ) * 12;

		// Workout Angles
		var arch = aim * (1 - aim);
		var startArch = (aim * aim) * (1 - aim);

		var endArch = aim * (1 - aim);
		endArch *= endArch;

		setup.Hands.Angles *= Rotation.FromRoll( arch * -35 ) * Rotation.FromPitch( startArch * 20 );
		setup.Hands.Offset += (setup.Rotation.Down * endArch * 4) + (setup.Rotation.Left * endArch * 8);
	}
}
