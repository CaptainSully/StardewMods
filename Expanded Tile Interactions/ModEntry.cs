global using SObject = StardewValley.Object;
global using SullySDVcore.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ExpandedTileInteractions
{
    // <summary>The mod entry point.</summary>
    public class ModEntry : Mod 
    {
        /*********
        ** Fields
        *********/
        /// <summary>Static reference to the mod instance for logging in other classes.</summary>
        internal static ModEntry Instance { get; set; }

        /// <summary>Logging tool.</summary>
        internal Log log;

        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>The mods unique ID.</summary>
        internal static string UID { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            log = new(this);
            UID = ModManifest.UniqueID;

            UpdateConfig();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ModConfig.SetUpModConfigMenu(Config, this);
        }

        /// <summary>Update the mod configuration.</summary>
        private void UpdateConfig()
        {
            Config = Helper.ReadConfig<ModConfig>();
            ModConfig.VerifyConfigValues(Config, this);
        }
    }
}