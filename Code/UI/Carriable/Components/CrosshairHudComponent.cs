using Sandbox;
using Sandbox.UI;
using Sandbox.Utility;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class CrosshairHudComponent : EntityHudComponent<RootPanel, Pawn>, IMutate<CameraSetup>
{
	protected override void OnCreateUI( RootPanel root )
	{
		base.OnCreateUI( root );
		m_Crosshair = root.AddChild<UI.Crosshair>();
		m_Crosshair.AddClass( "dm_pistol" );
	}

	private UI.Crosshair m_Crosshair;
	private AnimatedEntity m_Entity;

	[GameEvent.Client.Frame]
	private void OnFrame()
	{
		if ( !m_Entity.IsValid() )
			return;

		if ( m_Entity.GetAttachment( "muzzle" ) is not { } muzzle )
			return;

		var ray = Trace.Ray(
				muzzle.Position + muzzle.Rotation.Backward * 4,
				muzzle.Position + muzzle.Rotation.Forward * 512
			)
			.Ignore( Entity )
			.Run();

		m_Crosshair.ToWorld( ray.EndPosition );
	}

	[Listen]
	private void OnViewModelCreated( Event<CreatedViewModel> evt )
	{
		m_Entity = evt.Data.ViewModel;
	}

	[Listen]
	private void OnWeaponAttack( Event<WeaponFired> evt )
	{
		m_Crosshair.Fired( evt );
	}

	public void OnPostSetup( ref CameraSetup setup )
	{
		m_Crosshair.Style.Opacity = MathX.Lerp( 0.4f, 0, Easing.QuadraticInOut( setup.Hands.Aim ) );
	}
}
