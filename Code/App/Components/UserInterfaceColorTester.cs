using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Espionage;

// [SceneCamera.AutomaticRenderHook]
public sealed class UserInterfaceColorTester : RenderHook
{
	public UserInterfaceColorTester()
	{
		m_Color = Color.Random;
	}

	private Color m_Color;

	public override void OnStage( SceneCamera target, Stage renderStage )
	{
		base.OnStage( target, renderStage );

		if ( renderStage == Stage.AfterTransparent )
		{
			Graphics.Clear( m_Color, clearStencil: true );
		}

		if ( m_SinceRandom > 3 )
		{
			Randomise();
		}
	}

	private TimeSince m_SinceRandom;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Color GetNonMatchingRandomColor( Color current )
	{
		var target = current;
		while ( current == target )
		{
			target = Color.Random;
		}

		return target;
	}

	public void Randomise()
	{
		m_Color = GetNonMatchingRandomColor( m_Color );
		m_SinceRandom = 0;
	}
}
