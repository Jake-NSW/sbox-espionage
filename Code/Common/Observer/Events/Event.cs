﻿using System;
using Sandbox;

namespace Woosh.Common;

public readonly struct Event<T> where T : struct, IEventData
{
	public T Data { get; }

	public object From { get; }
	public DateTime TimeStamp { get; }

	public bool IsServer => Game.IsServer;
	public bool IsClient => Game.IsClient;
	public bool IsPredicted { get; }

	internal Event( T data, object from )
	{
		From = from;
		Data = data;

		IsPredicted = Prediction.Enabled && Prediction.CurrentHost != null;
		TimeStamp = DateTime.Now;
	}

	public static implicit operator T( Event<T> evt )
	{
		return evt.Data;
	}
}