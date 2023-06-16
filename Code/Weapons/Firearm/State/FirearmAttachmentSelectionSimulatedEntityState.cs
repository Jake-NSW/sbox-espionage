using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class FirearmAttachmentSelectionSimulatedEntityState : ObservableEntityComponent<Firearm>, ISimulatedEntityState<Firearm>, ISingletonComponent
{
	public bool TryEnter()
	{
		return false;
	}

	public bool Simulate( IClient cl )
	{
		return false;
	}

	public void OnStart()
	{
		throw new System.NotImplementedException();
	}

	public void OnFinish()
	{
		throw new System.NotImplementedException();
	}
}
