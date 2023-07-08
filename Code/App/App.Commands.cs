using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed partial class App
{
	public static class Commands
	{
		[ConCmd.Admin( "noclip" )]
		public static void PawnNoclip()
		{
			var pawn = ConsoleSystem.Caller.Pawn as Pawn;
			if ( pawn.Components.HasAny<WalkController>() )
				pawn.Components.Replace<WalkController, NoclipController>();
			else
				pawn.Components.Replace<NoclipController, WalkController>();
		}

		[ConCmd.Admin( "esp_ent_create" )]
		public static void CreateEntity( string className )
		{
			var pawn = ConsoleSystem.Caller.Pawn;
			var forward = pawn.AimRay.Forward;
			var start = pawn.AimRay.Position;
			var ray = Trace.Ray( start, start + forward * 128 ).Ignore( pawn ).Run();
			TypeLibrary.GetType( className ).Create<Entity>().Position = ray.EndPosition + -forward * 4;
		}
	}
}
