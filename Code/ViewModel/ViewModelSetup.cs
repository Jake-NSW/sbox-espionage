namespace Woosh.Espionage;

public ref struct ViewModelSetup
{
	public ViewModelSetup()
	{
		Aim = 0;
		Offset = Vector3.Zero;
		Angles = Rotation.Identity;
	}


	// Mutable

	public float Aim;

	public Vector3 Offset;
	public Rotation Angles;
}
