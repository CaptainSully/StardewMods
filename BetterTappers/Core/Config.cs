
namespace BetterTappers
{
    public class Config
    {
        // General options
        public bool DisableAllModEffects { get; set; } = false;
        public bool ChangeTapperTimes { get; set; } = true;
        public bool TappersUseQuality { get; set; } = true;
        public int TapperXP { get; set; } = 10;
        public bool GathererAffectsTappers { get; set; } = true;
        public bool BotanistAffectsTappers { get; set; } = true;


        // Options for regular tappers
        public float DaysForSyrups { get; set; } = 7f;
        public float DaysForSap { get; set; } = 1f;
        public float DaysForMushroom { get; set; } = 7f;

        // Options for hardwood tappers
        public bool OverrideHeavyTapperDefault { get; set; } = false;
        public float HeavyTapperMultiplier { get; set; } = 0.5f;
        public float DaysForSyrupsHeavy { get; set; } = 3.5f;
        public float DaysForSapHeavy { get; set; } = 0.5f;
        public float DaysForMushroomHeavy { get; set; } = 3.5f;

        // Quality options
        public bool ForageLevelAffectsQuality { get; set; } = true;
        public bool TimesHarvestedAffectsQuality { get; set; } = true;
        public bool TreeAgeAffectsQuality { get; set; } = true;
        internal int Formula { get; set; } = 0;
        internal int LvlCap { get; set; } = 0;

        // Debug mode
        public bool DebugMode { get; set; } = false;
        internal bool DebugTapper { get; set; } = false;
        internal bool DebugLogic { get; set; } = false;
        internal bool DebugPatcher { get; set; } = false;
        internal bool DebugMethods { get; set; } = false;

        //different outputs?
        //more outputs? (like 3-8 sap)

        public static void VerifyConfigValues(Config config, ModEntry mod)
        {
            bool invalidConfig = false;

            if (config.TapperXP < 0)
            {
                invalidConfig = true;
                config.TapperXP = 0;
            }

            if (config.DaysForSyrups < 0)
            {
                invalidConfig = true;
                config.DaysForSyrups = 7f;
            }

            if (config.DaysForSap < 0)
            {
                invalidConfig = true;
                config.DaysForSap = 1f;
            }

            if (config.DaysForMushroom < 0)
            {
                invalidConfig = true;
                config.DaysForMushroom = 7f;
            }

            if (config.HeavyTapperMultiplier < 0)
            {
                invalidConfig = true;
                config.HeavyTapperMultiplier = 0.5f;
            }

            if (config.DaysForSyrupsHeavy < 0)
            {
                invalidConfig = true;
                config.DaysForSyrupsHeavy = 3.5f;
            }

            if (config.DaysForSapHeavy < 0)
            {
                invalidConfig = true;
                config.DaysForSapHeavy = 0.5f;
            }

            if (config.DaysForMushroomHeavy < 0)
            {
                invalidConfig = true;
                config.DaysForMushroomHeavy = 3.5f;
            }

            if (invalidConfig)
            {
                Log.T("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }
        
        public static void SetUpModConfigMenu(Config config, ModEntry mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api == null) { return; }
            var manifest = mod.ModManifest;

            api.Register(manifest, () => config = new Config(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            // General mod settings. Some of these affect the other categories
            api.AddSectionTitle(manifest, text: () => "General");

            api.AddBoolOption(manifest, () => config.DisableAllModEffects, (bool val) => config.DisableAllModEffects = val,
                    name: () => "Disable this mods effects", tooltip: () => "Game will follow vanilla behaviour if true.\n'True' overrides ALL other settings.");
            api.AddBoolOption(manifest, () => config.ChangeTapperTimes, (bool val) => config.ChangeTapperTimes = val,
                    name: () => "Enable modified production times", tooltip: () => "Let tappers use modified product times.\n'False' overrides the production times settings.");
            api.AddBoolOption(manifest, () => config.TappersUseQuality, (bool val) => config.TappersUseQuality = val,
                    name: () => "Enable quality for tapper products", tooltip: () => "Lets tappers produce items with higher qualities.\n'False' overrides the quality section below.");
            api.AddNumberOption(manifest, () => config.TapperXP, (int val) => config.TapperXP = val,
                    name: () => "Tapper experience gain", tooltip: () => "Amount of experience gained for harvesting from tappers.\nMod default is 10, vanilla is 0.");
            api.AddBoolOption(manifest, () => config.GathererAffectsTappers, (bool val) => config.GathererAffectsTappers = val,
                    name: () => "Enable Gatherer perk on tappers", tooltip: () => "The gatherer foraging perk (vanilla) gives a chance for double foraged items.");
            api.AddBoolOption(manifest, () => config.GathererAffectsTappers, (bool val) => config.GathererAffectsTappers = val,
                    name: () => "Enable Botanist perk on tappers", tooltip: () => "The botanist foraging perk (vanilla) makes forage items always irridium quality.");


            // Production times for normal tappers
            api.AddSectionTitle(manifest, text: () => "Tappers",
                tooltip: () => "These options affect production time of tappers.\nThis section requires 'Enable modified production times' to be true.");

            api.AddNumberOption(manifest, () => config.DaysForSyrups, (float val) => config.DaysForSyrups = val,
                    name: () => "Days for maple/oak/pine trees", tooltip: () => "Number of days for regular tappers to produce on listed trees.\nVanilla is 5-9 depending on tree type.");
            api.AddNumberOption(manifest, () => config.DaysForSap, (float val) => config.DaysForSap = val,
                    name: () => "Days for mahogany trees", tooltip: () => "Number of days for regular tappers to produce on mahogany trees.\nVanilla is 1.");
            api.AddNumberOption(manifest, () => config.DaysForMushroom, (float val) => config.DaysForMushroom = val,
                    name: () => "Days for mushroom trees", tooltip: () => "Number of days for regular tappers to produce on mushroom trees.\nVanilla is varies heavily based on season.\nNote that rules for *which* mushroom is produced are not changed in any way.");


            // Production time for heavy tappers
            api.AddSectionTitle(manifest, text: () => "Heavy Tappers",
                tooltip: () => "These options affect production time of heavy tappers.\nThis section requires 'Enable modified production times' to be true.");

            api.AddNumberOption(manifest, () => config.HeavyTapperMultiplier, (float val) => config.HeavyTapperMultiplier = val,
                    name: () => "Heavy tapper time multiplier", tooltip: () => "Defaults to half normal tappers, which is the same as vanilla.\nThis gets overriden if the manual setting below is true.");
            api.AddBoolOption(manifest, () => config.OverrideHeavyTapperDefault, (bool val) => config.OverrideHeavyTapperDefault = val,
                    name: () => "Manually set times for heavy tappers", tooltip: () => "Uses the above setting if this is false, and ignores the next 3 settings.");
            api.AddNumberOption(manifest, () => config.DaysForSyrupsHeavy, (float val) => config.DaysForSyrupsHeavy = val,
                    name: () => "Days for maple/oak/pine trees", tooltip: () => "Number of days for heavy tappers to produce on listed trees.");
            api.AddNumberOption(manifest, () => config.DaysForSapHeavy, (float val) => config.DaysForSapHeavy = val,
                    name: () => "Days for mahogany trees", tooltip: () => "Number of days for heavy tappers to produce on mahogany trees.");
            api.AddNumberOption(manifest, () => config.DaysForMushroomHeavy, (float val) => config.DaysForMushroomHeavy = val,
                    name: () => "Days for mushroom trees", tooltip: () => "Number of days for heavy tappers to produce on mushroom trees.");


            // How to determine tapper product quality
            api.AddSectionTitle(manifest, text: () => "Tapper Product Quality");
            api.AddParagraph(manifest, text: () => "These options affect how output quality is determined. This section requires " +
                    "'Enable quality for tapper products' to be true. If all of these are false products will never have quality." +
                    "\nWith default settings, each of 'Forage level', 'Times harvested', and 'Tree age' are added together to determine the output. " +
                    "This gives a value between 0 and 6. On a 6 the quality will be irridium; otherwise the value is divided by 2 then rounded down and that is the quality. " +
                    "Vanilla qualities are 0 for normal, 1 for silver, 2 for gold, and 4 for irridium (yes it skips 3)\n");

            api.AddBoolOption(manifest, () => config.ForageLevelAffectsQuality, (bool val) => config.ForageLevelAffectsQuality = val,
                    name: () => "Forage level affects quality", tooltip: () => "Your level of foraging will affect the quality of tapper products.");
            api.AddBoolOption(manifest, () => config.TimesHarvestedAffectsQuality, (bool val) => config.TimesHarvestedAffectsQuality = val,
                    name: () => "Times harvested affects quality", tooltip: () => "Number of times a tapper has been harvested will affect the quality of its products.");
            api.AddBoolOption(manifest, () => config.TreeAgeAffectsQuality, (bool val) => config.TreeAgeAffectsQuality = val,
                    name: () => "Tree age affects quality", tooltip: () => "Tree age will affect the quality of tapper products.");

            api.AddSectionTitle(manifest, text: () => "\n\nDebug");
            api.AddBoolOption(manifest, () => config.DebugMode, (bool val) => config.DebugMode = val,
                    name: () => "This is for helping me test things, leave disabled.", tooltip: () => null);
        }
    }
}