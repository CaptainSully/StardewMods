using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BetterTappers
{
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

            Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
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
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.Event_LoadLate;
        }
        private void Event_LoadLate(object sender, OneSecondUpdateTickedEventArgs e)
        {
            this.Helper.Events.GameLoop.OneSecondUpdateTicked -= this.Event_LoadLate;

            if (this.LoadAPIs())
            {
                this.Initialise();
            }
        }
        private void Initialise()
        {
            DebugLog("Initialising mod data.", true);

            // Content
            //Translations.Initialise();
            BetterTappersConfig.SetUpModConfigMenu(Config, this);

            // Patches
            Patcher.PatchAll(this);

            // Events
            Helper.Events.GameLoop.DayStarted += delegate { BetterTappersLogic.IncreaseTreeAges(); };
        }
        private bool LoadAPIs()
        {
            DebugLog("Loading mod-provided APIs.", true);
            ISpaceCoreAPI spacecoreAPI = this.Helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore");
            if (spacecoreAPI == null)
            {
                // Skip patcher mod behaviours if we fail to load the objects
                ErrorLog($"Couldn't access mod-provided API for SpaceCore.{Environment.NewLine}Better Tappers will not be available, and no changes will be made.");
                return false;
            }

            spacecoreAPI.RegisterSerializerType(typeof(Tapper));

            return true;
        }
    }
}