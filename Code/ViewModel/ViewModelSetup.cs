using Sandbox;

namespace Woosh.Espionage;

public struct ViewModelSetup
{
	public ViewModelSetup()
	{
		Aim = 0;
		Offset = Vector3.Zero;
		Angles = Rotation.Identity;
	}

	public static ViewModelSetup Lerp( ViewModelSetup from, ViewModelSetup to, float t )
	{
		return new ViewModelSetup()
		{
			Aim = MathX.Lerp( from.Aim, to.Aim, t ),
			Offset = Vector3.Lerp( from.Offset, to.Offset, t ),
			Angles = Rotation.Lerp( from.Angles, to.Angles, t ),
		};
	}

	// Mutable

	public float Aim;

	public Vector3 Offset;
	public Rotation Angles;
}
