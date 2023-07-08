using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace Woosh.Common;

public static class LabelUtility
{
	public async static Task WriteOut( this Label label, string text, float delay = 0.05f )
	{
		label.Text = string.Empty;

		for ( var i = 0; i < text.Length; i++ )
		{
			if ( !label.IsValid() )
				return;

			if ( label.Text.Length > 0 )
				label.Text = label.Text.Remove( label.Text.Length - 1 );

			label.Text += text[i];

			if ( i != text.Length - 1 )
				label.Text += "_";

			await GameTask.DelaySeconds( delay );
		}

		label.Text = text;
	}
}
