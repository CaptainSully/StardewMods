using StardewValley.Tools;

namespace InfiniteWateringCan
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
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ModConfig.SetUpModConfigMenu(Config, this);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Config.InfiniteWater && Game1.player.CurrentTool is WateringCan can)
                can.WaterLeft = can.waterCanMax;
        }
    }
}