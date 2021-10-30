using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BetterTappers
{
    public class ModEntry : Mod
    {
        internal static IMonitor Logger { get; set; }
        internal static Config Config { get; set; }
        internal static string UID { get; set; }

        public override void Entry(IModHelper helper)
        {
            UID = ModManifest.UniqueID;
            Logger = Monitor;
            Config = Helper.ReadConfig<Config>();
            Config.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
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
            Log.T("Initialising mod data.");

            // Content
            //Translations.Initialise();
            Config.SetUpModConfigMenu(Config, this);

            // Patches
            Patcher.PatchAll(this);

            // Events
            Helper.Events.GameLoop.DayStarted += delegate { BetterTappersLogic.IncreaseTreeAges(); };
        }
        private bool LoadAPIs()
        {
            Log.T("Loading mod-provided APIs.");
            ISpaceCoreAPI spacecoreAPI = Helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore");
            if (spacecoreAPI == null)
            {
                // Skip patcher mod behaviours if we fail to load the objects
                Log.E($"Couldn't access mod-provided API for SpaceCore.{Environment.NewLine}Better Tappers will not be available, and no changes will be made.");
                return false;
            }

            spacecoreAPI.RegisterSerializerType(typeof(Tapper));

            return true;
        }
    }
}