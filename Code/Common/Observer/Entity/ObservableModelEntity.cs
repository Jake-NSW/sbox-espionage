using Sandbox;

namespace Woosh.Common;

public abstract class ObservableModelEntity : ModelEntity, IObservableEntity
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

	public override void OnNewModel( Model model )
	{
		base.OnNewModel( model );
		Events.Run( new ModelChanged( model ) );
	}
}
