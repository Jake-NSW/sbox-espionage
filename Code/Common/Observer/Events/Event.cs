using Sandbox;

namespace Woosh.Common;

public ref struct Event<T> where T : struct, IEventData
{
	public T Data { get; }

	public object From { get; }

	public bool IsServer => Game.IsServer;
	public bool IsClient => Game.IsClient;
	public bool IsPredicted { get; }

	internal Event( T data, object from )
	{
		IsPredicted = Prediction.Enabled && Prediction.CurrentHost != null;
		
		From = from;
		Data = data;
	}

	public bool Consumed => m_Consumed;
	private bool m_Consumed;

	public void Consume()
	{
		m_Consumed = true;
	}

	public static implicit operator T( Event<T> evt )
	{
		return evt.Data;
	}
}
