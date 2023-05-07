using Sandbox;
using Sandbox.ModelEditor;

namespace Woosh.Espionage.Data;

[GameData( "esp_display_info" )]
public sealed class ModelDisplayInfo
{
	public string Name { get; set; }
	public string Icon { get; set; }

	public DisplayInfo Info => new DisplayInfo() { Icon = Icon, Name = Name };
}
