using Sandbox;
using Woosh.Espionage;

namespace Woosh.Espionage;

public sealed partial class App
{
	public static class Commands
	{
		[ConCmd.Admin( "noclip" )]
		private static void PawnNoclip()
		{
			var pawn = ConsoleSystem.Caller.Pawn as Pawn;
			if ( pawn.Components.HasAny<WalkController>() )
				pawn.Components.Replace<WalkController, NoclipController>();
			else
				pawn.Components.Replace<NoclipController, WalkController>();
		}

		[ConCmd.Admin( "ent_create" )]
		private static void CreateEntity( string className )
		{
			var pawn = ConsoleSystem.Caller.Pawn;
			var forward = pawn.AimRay.Forward;
			var start = pawn.AimRay.Position;
			var ray = Trace.Ray( start, start + forward * 128 ).Ignore( pawn ).Run();
			TypeLibrary.GetType( className ).Create<Entity>().Position = ray.EndPosition + -forward * 4;
		}
	}
}
