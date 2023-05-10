using Sandbox;

namespace Woosh.Common;

public readonly struct ModelChanged : IEventData
{
	public Model Model { get; }

	public ModelChanged( Model model )
	{
		Model = model;
	}
}
