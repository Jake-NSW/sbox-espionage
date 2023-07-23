using System;
using System.Runtime.CompilerServices;
using Sandbox;
using Sandbox.Diagnostics;

namespace Woosh.Espionage;

public static class LogUtility
{

	public static TimedScope Measure( this Logger logger, [CallerMemberName] string message = null )
	{
		return new TimedScope( message );
	}

	public readonly struct TimedScope : IDisposable
	{
		private readonly string m_Message;
		private readonly RealTimeSince m_Since;

		public TimedScope( string message )
		{
			m_Message = message;
			m_Since = 0;
		}

		public void Dispose()
		{
			Log.Info( $"{m_Message} | {m_Since}" );
		}
	}
}
