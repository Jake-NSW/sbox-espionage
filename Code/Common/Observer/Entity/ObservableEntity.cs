using Sandbox;

namespace Woosh.Common;

public abstract class ObservableEntity : Entity, IObservableEntity
{
	private Dispatcher m_Events;
	public Dispatcher Events => m_Events ??= new Dispatcher();

	protected override void OnComponentAdded( EntityComponent component )
	{
		base.OnComponentAdded( component );
		Events.Run( new ComponentAdded( component ) );
	}

	protected override void OnComponentRemoved( EntityComponent component )
	{
		base.OnComponentRemoved( component );
		Events.Run( new ComponentRemoved( component ) );
	}
}
