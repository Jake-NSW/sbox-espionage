using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelProjectilePredictor : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{

	public void OnPostSetup( ref CameraSetup setup )
	{
		const string name = "muzzle";

		// Get the muzzle attachment
		var attachment = Entity.GetAttachment( name );
		if ( !attachment.HasValue )
			return;

		var muzzle = attachment.Value;

		var helper = new ProjectMovementHelper();
		var samples = helper.Sample( muzzle.Position + setup.Hands.Offset, (muzzle.Rotation * setup.Hands.Angles).Forward, 250, 0.0009f, 1f, 0.01f );

		var time = Input.Pressed( "shoot" ) ? 5 : 0;


		for ( int i = 1; i < samples.Length; i++ )
		{
			DebugOverlay.Sphere( samples[i - 1], 0.2f, Color.Red, duration: time );
			DebugOverlay.Line( samples[i - 1], samples[i], Color.Black, duration: time );

			var ray = Trace.Ray( samples[i - 1], samples[i] )
				.Ignore( Game.LocalPawn )
				.Size( 1 )
				.Run();

			if ( Input.Pressed( "shoot" ) )
			{
				var start = muzzle.Position + setup.Hands.Offset;
				DebugOverlay.Line( start, start + (muzzle.Rotation * setup.Hands.Angles).Forward * 15000, Color.White, duration: time );
			}

			if ( ray.Hit )
			{
				DebugOverlay.Sphere( ray.EndPosition, 4f, Color.Blue, duration: time );
				break;
			}
		}
	}
}
