
namespace StartWithGreenhouse
{
    // <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>Static reference to the mod instance for logging in other classes.</summary>
        internal static ModEntry Instance { get; set; }
        /// <summary>Logging tool.</summary>
        internal Log log;
        /// <summary>The mod configuration.</summary>
        internal ModConfig Config { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            // initalize fields
            Instance = this;
            log = new(this);

            // load config
            Config = Helper.ReadConfig<ModConfig>();

            // hook events
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ModConfig.SetUpModConfigMenu(Config, this);
        }

        /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
                return;
            if (!Config.DisableAllModEffects && !Game1.player.mailReceived.Contains("ccPantry"))
            {
                Game1.player.mailReceived.Add("ccPantry");
                log.T("Pantry flag set (Greenhouse unlocked).");
            }
        }
    }
}