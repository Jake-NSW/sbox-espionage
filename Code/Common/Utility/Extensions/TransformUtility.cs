namespace Woosh.Common;

public static class TransformUtility
{
	public static Ray ToRay( this Transform transform )
	{
		return new Ray( transform.Position, transform.Rotation.Forward );
	}
}
