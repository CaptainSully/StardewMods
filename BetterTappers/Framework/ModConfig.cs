
namespace BetterTappers
{
    public class ModConfig
    {
        private static readonly Log log = ModEntry.Instance.log;

        /*********
        ** Accessors
        *********/

        // Tapper general settings
        /// <summary>Whether to use this mods tapper production times.</summary>
        public bool ChangeTapperTimes { get; set; } = true;
        /// <summary>Whether tappers should only produce overnight.</summary>
        //public bool TappersOnlyOvernight { get; set; } = false;
        /// <summary>Whether to use this mods tapper production amounts.</summary>
        public bool ChangeTapperAmounts { get; set; } = true;
        /// <summary>Whether to have tappers give xp.</summary>
        public int TapperXP { get; set; } = 3;
        /// <summary>Whether the gatherer profession should affect tappers.</summary>
        public bool GathererAffectsTappers { get; set; } = true;
        /// <summary>Whether the botanist profession should affect tappers.</summary>
        public bool BotanistAffectsTappers { get; set; } = true;
        /// <summary>Whether mushroom and fern tree tappers should produce during winter.</summary>
        public bool ProduceInWinter { get; set; } = true;

        // Quality settings
        public string QualitySetting { get; set; } = "Foraging Level";
        public int TreeAgeIncrement { get; set; } = 84;

        // Tree settings
        /// <summary>Whether trees should grow during winter.</summary>
        public bool GrowsInWinter { get; set; } = true;
        /// <summary>Whether nearby trees should prevent tree growth.</summary>
        public bool NearbyTreesBlockGrowth { get; set; } = true;
        /// <summary>Whether mushroom and fern trees should be stumps during winter.</summary>
        public bool IsStumpDuringWinter { get; set; } = false;
        /// <summary>Whether coconuts should be plantable to grow palm trees.</summary>
        public bool CoconutPalmSeed { get; set; } = true;

        // Production time and amounts for regular tappers
        public int DaysForResin { get; set; } = 7;
        public int AmountOfResin { get; set; } = 1;
        public int DaysForSyrup { get; set; } = 7;
        public int AmountOfSyrup { get; set; } = 1;
        public int DaysForTar { get; set; } = 7;
        public int AmountOfTar { get; set; } = 1;
        public int DaysForOil { get; set; } = 7;
        public int AmountOfOil { get; set; } = 1;
        public int DaysForSap { get; set; } = 1;
        public int AmountOfSap { get; set; } = 6;
        public int DaysForMushroom { get; set; } = 7;
        public int AmountOfMushrooms { get; set; } = 1;
        public int DaysForMoss { get; set; } = 2;
        public int AmountOfMoss { get; set; } = 1;
        public int DaysForFern { get; set; } = 2;
        public int AmountOfFerns { get; set; } = 1;
        public int DaysForMystic { get; set; } = 7;
        public int AmountOfMysticSyrup { get; set; } = 1;

        // Options for heavy tappers
        //public float HeavyTapperMultiplier { get; set; } = 0.5f;

        // Debug mode
        public bool DebugMode { get; set; } = false;
        internal bool DebugMethods { get; set; } = false;
        internal bool DebugLogic { get; set; } = false;
        internal bool DebugPatcher { get; set; } = false;


        /*********
        ** Public methods
        *********/
        public static void SetUpModConfigMenu(ModConfig config, Mod mod)
        {
            // Get the Generic Mod Config Menu API
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null) { return; }
            var manifest = mod.ModManifest;

            // Register the Generic Mod Config Menu API
            api.Register(manifest, () => config = new ModConfig(), delegate { mod.Helper.WriteConfig(config); });


            // Tapper general settings. Some of these affect the other categories
            api.AddSectionTitle(manifest, text: () => "Tapper general settings");

            api.AddBoolOption(manifest, () => config.ChangeTapperTimes, (bool val) => config.ChangeTapperTimes = val,
                    name: () => "Enable modified production times",
                    tooltip: () =>  "Let tappers use modified product times." +
                                    "\n'False' overrides the production times settings.");
            /*
            api.AddBoolOption(manifest, () => config.TappersOnlyOvernight, (bool val) => config.TappersOnlyOvernight = val,
                    name: () => "Tappers only finish overnight",
                    tooltip: () => "Whether tappers will only finish producing overnight." +
                                    "\nDefault false. Vanilla true.");
            */
            api.AddBoolOption(manifest, () => config.ChangeTapperAmounts, (bool val) => config.ChangeTapperAmounts = val,
                    name: () => "Enable modified production amounts",
                    tooltip: () =>  "Let tappers use modified product amounts." +
                                    "\n'False' overrides the production amounts settings.");
            api.AddNumberOption(manifest, () => config.TapperXP, (int val) => config.TapperXP = val,
                    name: () => "Tapper experience gain",
                    tooltip: () => "Amount of experience gained for harvesting from tappers." +
                                    "\nDefault is 3, vanilla is 0.",
                    0, 50, 1);
            api.AddBoolOption(manifest, () => config.GathererAffectsTappers, (bool val) => config.GathererAffectsTappers = val,
                    name: () => "Enable Gatherer perk on tappers",
                    tooltip: () => "The gatherer foraging perk (vanilla) gives a chance for double foraged items.");
            api.AddBoolOption(manifest, () => config.BotanistAffectsTappers, (bool val) => config.BotanistAffectsTappers = val,
                    name: () => "Enable Botanist perk on tappers",
                    tooltip: () => "The botanist foraging perk (vanilla) makes forage items always irridium quality." +
                                    "\nOnly applies if 'Quality setting' (see below) is not 'None'");
            api.AddBoolOption(manifest, () => config.ProduceInWinter, (bool val) => config.ProduceInWinter = val,
                    name: () => "Produce in winter",
                    tooltip: () => "Whether mushroom and fern tree tappers should produce in winter." +
                                    "\nDefault true. Vanilla is false.");


            // How to determine tapper product quality
            api.AddSectionTitle(manifest, text: () => "Tapper Product Quality",
                tooltip: () => "These options affect tapper output quality.");

            api.AddTextOption(manifest, () => config.QualitySetting, (string val) => config.QualitySetting = val,
                    name: () => "Quality setting",
                    allowedValues: ["None", "Foraging Level", "Tree Age"],
                    tooltip: () => "How the quality of tapper outputs should be decided." +
                                    "\n'Foraging Level' is calculated as per any vanilla forage." +
                                    "\nNote: sap and moss are excluded from quality.");
            api.AddNumberOption(manifest, () => config.TreeAgeIncrement, (int val) => config.TreeAgeIncrement = val,
                    name: () => "Age (days) for quality increase",
                    tooltip: () => "Only applies if 'Quality Setting' is 'Tree Age'" +
                                    "\nHow old a tree needs to be (days) to increase the output quality by 1." +
                                    "\nDefault 84 days (3 months). This means irridium quality from trees at least 252 days old.",
                                    1);


            // Tree general settings
            api.AddSectionTitle(manifest, text: () => "Tree general settings",
                tooltip: () => "These options affect wild tree behaviour." +
                                "\nThese do NOT affect fruit trees.");
            api.AddBoolOption(manifest, () => config.NearbyTreesBlockGrowth, (bool val) => config.NearbyTreesBlockGrowth = val,
                    name: () => "Nearby trees block growth",
                    tooltip: () => "Whether nearby wild trees prevent tree growth." +
                                    "\nVanilla true.");
            api.AddBoolOption(manifest, () => config.GrowsInWinter, (bool val) => config.GrowsInWinter = val,
                    name: () => "Trees grow in winter",
                    tooltip: () => "Whether wild trees grow during winter." +
                                    "\nVanilla false.");
            api.AddBoolOption(manifest, () => config.IsStumpDuringWinter, (bool val) => config.IsStumpDuringWinter = val,
                    name: () => "Become stumps during winter",
                    tooltip: () => "Whether mushroom and fern trees become stumps during winter." +
                                    "Note: false does not make them produce during winter. See 'Produce in winter' above." +
                                    "\nVanilla true.");
            api.AddBoolOption(manifest, () => config.CoconutPalmSeed, (bool val) => config.CoconutPalmSeed = val,
                    name: () => "Enable planting palm trees",
                    tooltip: () => "Lets you plant coconuts to grow palm trees." +
                                    "\nVanilla false.");


            // Production times for normal tappers
            api.AddSectionTitle(manifest, text: () => "Tapper Timings",
                tooltip: () => "These options affect production time of tappers." +
                                "\nThis section requires 'Enable modified production times' to be true.");

            api.AddNumberOption(manifest, () => config.DaysForResin, (int val) => config.DaysForResin = val,
                    name: () => "Days for oak trees",
                    tooltip: () => "Number of days for regular tappers to produce on oak trees.\nVanilla 7.",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.DaysForSyrup, (int val) => config.DaysForSyrup = val,
                    name: () => "Days for maple trees",
                    tooltip: () => "Number of days for regular tappers to produce on maple trees.\nVanilla is 9.",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.DaysForTar, (int val) => config.DaysForTar = val,
                    name: () => "Days for pine trees",
                    tooltip: () => "Number of days for regular tappers to produce on pine trees.\nVanilla is 5.",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.DaysForOil, (int val) => config.DaysForOil = val,
                    name: () => "Days for palm trees",
                    tooltip: () => "Number of days for regular tappers to produce oil from palm trees." +
                                    "\nVanilla palm trees do not produce.\nDefault is 7",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.DaysForSap, (int val) => config.DaysForSap = val,
                    name: () => "Days for mahogany trees",
                    tooltip: () => "Number of days for regular tappers to produce on mahogany trees.\nVanilla is 1.",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.DaysForMushroom, (int val) => config.DaysForMushroom = val,
                    name: () => "Days for mushroom trees",
                    tooltip: () => "Number of days for regular tappers to produce on mushroom trees." +
                                    "\nVanilla is 1 or 2 or not at all based on season." +
                                    "\nNote that rules for *which* mushroom is produced are not changed in any way.",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.DaysForMoss, (int val) => config.DaysForMoss = val,
                    name: () => "Days for green rain trees",
                    tooltip: () => "Number of days for regular tappers to produce moss from green rain trees." +
                                    "\nVanilla doesn't do this.",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.DaysForFern, (int val) => config.DaysForFern = val,
                    name: () => "Days for fern trees",
                    tooltip: () => "Number of days for regular tappers to produce on green rain fern trees." +
                                    "\nVanilla is 2.",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.DaysForMystic, (int val) => config.DaysForMystic = val,
                    name: () => "Days for mystic trees",
                    tooltip: () => "Number of days for regular tappers to produce on mystic trees.\nVanilla is 7.",
                    1, 50, 1);


            // Production amounts for normal tappers
            api.AddSectionTitle(manifest, text: () => "Tapper Amounts",
                tooltip: () => "These options affect production amounts of tappers." +
                                "\nThis section requires 'Enable modified production amounts' to be true.");

            api.AddNumberOption(manifest, () => config.AmountOfResin, (int val) => config.AmountOfResin = val,
                    name: () => "Amount of oak resin per harvest",
                    tooltip: () => null,
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.AmountOfSyrup, (int val) => config.AmountOfSyrup = val,
                    name: () => "Amount of maple syrup per harvest",
                    tooltip: () => null,
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.AmountOfTar, (int val) => config.AmountOfTar = val,
                    name: () => "Amount of pine tar per harvest",
                    tooltip: () => null,
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.AmountOfOil, (int val) => config.AmountOfOil = val,
                    name: () => "Amount of oil per harvest",
                    tooltip: () => "Default is 1, vanilla is 0",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.AmountOfSap, (int val) => config.AmountOfSap = val,
                    name: () => "Amount of sap per harvest",
                    tooltip: () => "Default is 6, vanilla is 3-8 (or an average of 5.5)",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.AmountOfMushrooms, (int val) => config.AmountOfMushrooms = val,
                    name: () => "Amount of mushrooms per harvest",
                    tooltip: () => null,
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.AmountOfMoss, (int val) => config.AmountOfMoss = val,
                    name: () => "Amount of moss per harvest",
                    tooltip: () => "Default is 1, vanilla is 0",
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.AmountOfFerns, (int val) => config.AmountOfFerns = val,
                    name: () => "Amount of ferns per harvest",
                    tooltip: () => null,
                    1, 50, 1);
            api.AddNumberOption(manifest, () => config.AmountOfMysticSyrup, (int val) => config.AmountOfMysticSyrup = val,
                    name: () => "Amount of mystic syrup per harvest",
                    tooltip: () => null,
                    1, 50, 1);


            // Production time for heavy tappers
            /*
            api.AddSectionTitle(manifest, text: () => "Heavy Tappers",
                tooltip: () => "These options affect production time of heavy tappers.\nThis section requires 'Enable modified production times' to be true.");

            api.AddNumberOption(manifest, () => config.HeavyTapperMultiplier, (float val) => config.HeavyTapperMultiplier = val,
                    name: () => "Heavy tapper time multiplier",
                    tooltip: () => "Vanilla is 0.5, or half the time of normal tappers.",
                    0f, 1f, 0.05f);
            */


            // Debuging
            api.AddParagraph(manifest, text: () => " ");
            api.AddSectionTitle(manifest, text: () => "Debuging");
            api.AddBoolOption(manifest, () => config.DebugMode, (bool val) => config.DebugMode = val,
                    name: () => "Debug mode",
                    tooltip: () => "This is for helping me debug/test things.\nEnable only if you're trying to do the same." +
                                    "\nThis is very spammy.");
        }
    }
}