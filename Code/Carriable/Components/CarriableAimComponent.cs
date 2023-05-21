using Sandbox;
using Sandbox.Utility;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed partial class CarriableAimComponent : ObservableEntityComponent<ICarriable>, IMutateCameraSetup
{
	public Easing.Function Easing => Sandbox.Utility.Easing.QuadraticInOut;

	public bool IsAiming => n_IsAiming;
	[Net, Predicted, Local] private bool n_IsAiming { get; set; }
	[Net, Predicted, Local] private float n_AimDelta { get; set; }
	private float AimSpeed => 2.5f;

	public void Simulate( IClient cl )
	{
		if ( CanAim( false ) )
		{
			ToggleAim( false );
		}
	}

	[Predicted] private bool p_LastState { get; set; }

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

		n_AimDelta = Easing( m_Delta );

		setup.Hands.Aim = n_AimDelta;
		setup.FieldOfView -= n_AimDelta * 20;

		DebugOverlay.ScreenText( $"IsAiming - {IsAiming}\nDelta = {n_AimDelta}", new Vector2( 30, 512 ) );
	}
}
