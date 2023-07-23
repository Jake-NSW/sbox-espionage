using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class FirearmAttachmentSelectionEntityState : ObservableEntityComponent<Firearm>, IEntityState<Firearm>, ISingletonComponent
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
