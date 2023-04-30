using Sandbox;

namespace Woosh.Espionage;

public sealed class ViewModelTuckEffect : IViewModelEffect
{
	private float m_LastTuck;

	public bool Update( ref ViewModelSetup setup )
	{
		string name = "muzzle";

		// Get the muzzle attachment
		var attachment = setup.Entity.GetAttachment( name );
		if ( !attachment.HasValue )
			return false;

		var muzzle = attachment.Value;

		// Get girth
		const float girth = 32;

		// Start Guntuck
		var start = muzzle.Position + muzzle.Rotation.Backward * Vector3.DistanceBetween( setup.Initial.Position - muzzle.Rotation.Backward / 4, muzzle.Position ) - (setup.Initial.Rotation.Backward * girth / 2.25f);
		var end = muzzle.Position + (muzzle.Rotation.Forward * 4);

		var tr = Trace.Ray( start, end )
			.Ignore( setup.Owner )
			.Ignore( setup.Entity )
			.Size( 1 )
			.Run();

		var offset = tr.Distance - Vector3.DistanceBetween( start, end );
		m_LastTuck = m_LastTuck.LerpTo( offset, 8 * Time.Delta );

		// Finish Guntuck
		setup.Position += setup.Initial.Rotation.Backward * -m_LastTuck;
		setup.Position += setup.Initial.Rotation.Down * -m_LastTuck / 4;
		return false;
	}

	public void Register( IDispatchRegistryTable table ) { }
	public void Unregister( IDispatchRegistryTable table ) { }
}
