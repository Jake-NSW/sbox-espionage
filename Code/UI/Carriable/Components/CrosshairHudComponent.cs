using Sandbox;
using Sandbox.UI;
using Sandbox.Utility;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class CrosshairHudComponent : EntityHudComponent<PawnEntity>, IMutate<CameraSetup>
{
	protected override Panel OnCreateUI()
	{
		var root = CreateFullscreenPanel();
		m_Crosshair = root.AddChild<UI.Crosshair>();
		m_Crosshair.AddClass( "dm_pistol" );
		return root;
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
		if ( !evt.Data.ViewModel.GetAttachment( "muzzle" ).HasValue )
			return;

		m_Entity = evt.Data.ViewModel;
	}

	[Listen]
	private void OnWeaponAttack( Event<WeaponFired> evt )
	{
		m_Crosshair.Fired( evt );
	}

	public void OnPostSetup( ref CameraSetup setup )
	{
		if ( !m_Entity.IsValid() || !m_Entity.GetAttachment( "muzzle" ).HasValue )
		{
			m_Crosshair.Style.Opacity = 0;
			return;
		}


		m_Crosshair.Style.Opacity = MathX.Lerp( 0.4f, 0, Easing.QuadraticInOut( setup.Hands.Aim ) );
	}
}
