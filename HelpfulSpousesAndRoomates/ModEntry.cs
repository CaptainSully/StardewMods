
namespace HelpfulSpousesAndRoomates
{
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
            I18n.Init(helper.Translation);

            // load config
            Config = Helper.ReadConfig<ModConfig>();

            // hook events
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.GameLoop.DayEnding += delegate { log.D("Day ending", Config.Debug); Chores.ResetChores(); };
            Helper.Events.GameLoop.ReturnedToTitle += delegate { log.D("Return to title", Config.Debug); Chores.ResetChores(); };

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

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Characters/Dialogue/ChoreDialogue"))
            {
                e.LoadFrom(
                    () => {
                        return new Dictionary<string, string>
                        {
                            // debug/test
                            ["Debug_Dialogue"] = I18n.DebugDialogue(),
                            // vanilla changes
                            ["Feed_Pet"] = I18n.D_FeedPet(),
                            ["Multi_Feed_Pet"] = I18n.D_MultiFeedPet(),
                            ["Sunny_Breakfast"] = I18n.D_SunnyBreakfast(),
                            ["Rainy_Breakfast"] = I18n.D_RainyBreakfast(),
                            // custom chores
                            ["Pet_Pet"] = I18n.D_PetPet(),
                            ["Multi_Pet_Pet"] = I18n.D_MultiPetPet(),
                            ["Combined_Pet"] = I18n.D_CombinedPet(),
                            ["Multi_Combined_Pet"] = I18n.D_MultiCombinedPet(),
                            ["Pet_Animals"] = I18n.D_PetAnimals(),
                            ["Combined_Animals"] = I18n.D_CombinedAnimals()
                        };
                    },
                    AssetLoadPriority.Medium
                );
            }
        }
    }
}
