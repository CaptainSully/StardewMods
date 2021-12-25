using StardewModdingAPI;
using System;

namespace BetterTappers
{
	public static class Log
	{
		public static void D(string str, bool debug)
		{
			if (debug) { ModEntry.Instance.Monitor.Log(str, LogLevel.Debug); }
		}
		public static void T(string str)
		{
			ModEntry.Instance.Monitor.Log(str, LogLevel.Trace);
		}
		public static void E(string str)
		{
			ModEntry.Instance.Monitor.Log(str, LogLevel.Alert);
		}
		public static void E(string str, Exception e)
		{
			string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";
			ModEntry.Instance.Monitor.Log(str + errorMessage, LogLevel.Error);
		}
		public static void I(string str)
		{
			ModEntry.Instance.Monitor.Log(str, LogLevel.Info);
		}
		public static void A(string str)
		{
			ModEntry.Instance.Monitor.Log(str, LogLevel.Alert);
		}
		public static void W(string str)
		{
			ModEntry.Instance.Monitor.Log(str, LogLevel.Warn);
		}
	}
}