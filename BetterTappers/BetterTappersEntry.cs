namespace BetterTappers
{
    using System;
    using StardewModdingAPI;

    public class BetterTappersEntry : Mod
    {
        public static IMonitor Logger { get; set; }
        public static BetterTappersConfig Config { get; set; }
        public static string UID { get; set; }

        public override void Entry(IModHelper helper)
        {
            UID = ModManifest.UniqueID;
            Logger = Monitor;
            Config = Helper.ReadConfig<BetterTappersConfig>();
            BetterTappersConfig.VerifyConfigValues(Config, this);
        
            Helper.Events.GameLoop.GameLaunched += delegate { BetterTappersConfig.SetUpModConfigMenu(Config, this); };
            Helper.Events.GameLoop.DayStarted += delegate { BetterTappersLogic.IncreaseTreeAges(); };

            Patcher.PatchAll(this);
        }
        public static void DebugLog(object o, bool debug)
        {
            if (debug)
            {
                Logger.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
            }
        }
        public static void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();
            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Logger.Log(baseMessage + errorMessage, LogLevel.Error);
        }
    }
}