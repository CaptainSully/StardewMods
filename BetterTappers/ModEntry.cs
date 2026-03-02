
namespace BetterTappers
{
    // <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>Static reference to the mod instance for logging in other classes.</summary>
        internal static ModEntry Instance { get; set; }
        /// <summary>Logging tool.</summary>
        internal Log log;
        /// <summary>The mod configuration.</summary>
        internal static ModConfig Config { get; set; }
        /// <summary>The mods unique id.</summary>
        internal static string UID { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            // initialize fields
            Instance = this;
            log = new(this);
            UID = ModManifest.UniqueID;

            // initialize translation helper
            //I18n.Init(helper.Translation);

            // load config
            Config = Helper.ReadConfig<ModConfig>();

            // hook events
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.DayStarted += delegate { CoreLogic.IncreaseTreeAges(); };
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            log.T("Initialising mod data.");

            // Patches
            Patcher.PatchAll();

            // Load APIs
            LoadAPIs();
        }

        /// <summary>Load other mod APIs.</summary>
        private void LoadAPIs()
        {
            log.T("Loading mod-provided APIs.");

            // setup GMCM
            ModConfig.SetUpModConfigMenu(Config, this);
        }
    }
}