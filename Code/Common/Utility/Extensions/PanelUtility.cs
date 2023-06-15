using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;
using Sandbox.UI;

namespace Woosh.Common;

public static class PanelUtility
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void ToEntity( this Panel panel, ModelEntity entity, int padding = 0 )
	{
		if ( !entity.IsValid() )
		{
			return;
		}

		Rect rect = default;
		var bounds = entity.CollisionBounds;
		bounds = bounds.Translate( entity.Position );

		var corners = bounds.Corners.ToArray();
		for ( var i = 0; i < corners.Length; i++ )
		{
			var pos = corners[i].RotateAround( entity.Position, entity.Rotation ).ToScreen();

			if ( rect == default )
			{
				rect = new Rect( pos );
				continue;
			}

			rect.Add( pos );
		}

		if ( padding != 0 )
		{
			rect = rect.Shrink( padding / Screen.Width, padding / Screen.Height );
		}

		panel.Style.Left = Length.Fraction( rect.Left );
		panel.Style.Top = Length.Fraction( rect.Top );
		panel.Style.Width = Length.Fraction( rect.Width );
		panel.Style.Height = Length.Fraction( rect.Height );
		panel.Style.Dirty();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void ToBounds( this Panel panel, BBox bounds, int padding = 0, bool allowOffScreen = true )
	{
		Rect rect = default;

		var corners = bounds.Corners.ToArray();
		for ( var i = 0; i < corners.Length; i++ )
		{
			var pos = corners[i].ToScreen();

			if ( !allowOffScreen && (pos.x < 0 || pos.y < 0) )
			{
				continue;
			}

			if ( rect == default )
			{
				rect = new Rect( pos );
				continue;
			}

			rect.Add( pos );
		}

		if ( padding != 0 )
		{
			rect = rect.Shrink( padding / Screen.Width, padding / Screen.Height );
		}

		panel.Style.Left = Length.Fraction( rect.Left );
		panel.Style.Top = Length.Fraction( rect.Top );
		panel.Style.Width = Length.Fraction( rect.Width );
		panel.Style.Height = Length.Fraction( rect.Height );
		panel.Style.Dirty();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void ToWorld( this Panel panel, Vector3 pos )
	{
		var screen = pos.ToScreen();

		if ( screen.z < 0 )
		{
			return;
		}

		panel.Style.Left = Length.Fraction( screen.x );
		panel.Style.Top = Length.Fraction( screen.y );
		panel.Style.Dirty();
	}

	public async static void WriteOut( this Label label, string text, float delay = 0.04f )
	{
		label.Text = string.Empty;

		for ( var i = 0; i < text.Length; i++ )
		{
			if ( label.Text.Length > 0 )
				label.Text = label.Text.Remove( label.Text.Length - 1 );

			label.Text += text[i];

			if ( i != text.Length - 1 )
				label.Text += "_";

			await GameTask.DelaySeconds( delay );
		}

		label.Text = text;
	}

	public async static void BackspaceOut( this Label label, float delay = 0.04f )
	{
		var length = label.Text.Length;

		for ( int i = 0; i < length; i++ )
		{
			if ( label.Text.Length > 2 )
				label.Text = "_" + label.Text.Substring( 2 );
			else
				label.Text = label.Text.Substring( 1 );

			await GameTask.DelaySeconds( delay );
		}

		label.Text = string.Empty;
	}
}

