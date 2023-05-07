namespace Woosh.Espionage;

public interface IMutateInputContext
{
	void OnPostInputBuild( ref InputContext context );
}

public ref struct InputContext
{
	public Angles ViewAngles;
	public Vector3 InputDirection;
}
