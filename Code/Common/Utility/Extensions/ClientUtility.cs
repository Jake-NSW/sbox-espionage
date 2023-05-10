using Sandbox;

namespace Woosh.Common;

public static class ClientUtility
{
	public static T Possess<T>( this IClient client, Transform spawn = default ) where T : Entity, new()
	{
		var pawn = new T();
		client.Pawn = pawn;
		pawn.Transform = spawn.WithScale( 1 );
		return pawn;
	}
}
