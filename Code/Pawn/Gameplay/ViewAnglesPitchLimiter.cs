using Sandbox;

namespace Woosh.Espionage;

public sealed partial class ViewAnglesPitchLimiter : EntityComponent, IMutate<InputContext>
{
	[Net] public int Up { get; set; }
	[Net] public int Down { get; set; }

	public ViewAnglesPitchLimiter() { }

	public ViewAnglesPitchLimiter( int down, int up )
	{
		Down = down;
		Up = up;
	}

	void IMutate<InputContext>.OnPostSetup( ref InputContext setup )
	{
		setup.ViewAngles.pitch = setup.ViewAngles.pitch.Clamp( -Up, Down );
	}
}
