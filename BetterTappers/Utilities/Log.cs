using System;

namespace BetterTappers
{
	public static class Log
	{
		public static void D(string str, bool debug)
		{
			if (debug)
				ModEntry.Logger.Log(str, StardewModdingAPI.LogLevel.Debug);
		}
		public static void A(string str)
		{
			ModEntry.Logger.Log(str, StardewModdingAPI.LogLevel.Alert);
		}
		public static void E(string str)
		{
			ModEntry.Logger.Log(str, StardewModdingAPI.LogLevel.Alert);
		}
		public static void E(string str, Exception e)
		{
			string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";
			ModEntry.Logger.Log(str + errorMessage, StardewModdingAPI.LogLevel.Error);
		}
		public static void I(string str)
		{
			ModEntry.Logger.Log(str, StardewModdingAPI.LogLevel.Info);
		}
		public static void T(string str)
		{
			ModEntry.Logger.Log(str, StardewModdingAPI.LogLevel.Trace);
		}
		public static void W(string str)
		{
			ModEntry.Logger.Log(str, StardewModdingAPI.LogLevel.Warn);
		}
	}
}