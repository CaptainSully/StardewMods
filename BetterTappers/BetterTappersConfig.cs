namespace BetterTappers
{
    using System;
    using StardewModdingAPI;

    public interface IGenericModConfigMenuAPI
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void AddParagraph(IManifest mod, Func<string> text);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, string fieldId = null);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, string fieldId = null);
     }

    public class BetterTappersConfig
    {
        //general options
        public bool UseBetterTappers { get; set; } = true;
        public bool Stackable { get; set; } = true;
        public bool TapperQuality { get; set; } = true;
        public int TapperXP { get; set; } = 10;
        public bool GathererAffectsTappers { get; set; } = true;


        //options for regular tappers
        public float DaysForSyrups { get; set; } = 7f;
        public float DaysForSap { get; set; } = 1f;
        public float DaysForMushroom { get; set; } = 7f;

        //options for hardwood tappers
        public float HeavyTapperMultiplier = 0.5f;
        public bool OverrideHeavyTapperDefault = false;
        public float DaysForSyrupsHeavy { get; set; } = 3.5f;
        public float DaysForSapHeavy { get; set; } = 0.5f;
        public float DaysForMushroomHeavy { get; set; } = 3.5f;

        //quality options
        public bool UseVanillaBotanist { get; set; } = false;
        public bool BotanistAffectsQuality { get; set; } = true;
        public bool ForageLevelAffectsQuality { get; set; } = true;
        public bool TimesHarvestedAffectsQuality { get; set; } = true;
        public bool TreeAgeAffectsQuality { get; set; } = true;

        //different outputs?
        //more outputs? (like 3-8 sap)

        public static void VerifyConfigValues(BetterTappersConfig config, BetterTappersEntry mod)
        {
            bool invalidConfig = false;

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
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        public static void SetUpModConfigMenu(BetterTappersConfig config, BetterTappersEntry mod)
        {
            IGenericModConfigMenuAPI api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api == null) { return; }
            var manifest = mod.ModManifest;

            api.Register(manifest, () => config = new BetterTappersConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            //General mod settings. Some of these affect the other categories
            api.AddSectionTitle(manifest, text: () => "General");

            api.AddBoolOption(manifest, () => config.UseBetterTappers, (bool val) => config.UseBetterTappers = val,
                    name: () => "Use this mods configurations", tooltip: () => "Game will follow vanilla behavour if false.\nThis overrides ALL other settings.");
            api.AddBoolOption(manifest, () => config.Stackable, (bool val) => config.Stackable = val,
                    name: () => "Make tappers stackable", tooltip: () => "Lets tappers stack in your inventory.\nStack size is 999.");
            api.AddBoolOption(manifest, () => config.TapperQuality, (bool val) => config.TapperQuality = val,
                    name: () => "Enable quality for tapper products", tooltip: () => "Lets tappers produce items with higher qualities.\nThis enables the quality section below.");
            api.AddNumberOption(manifest, () => config.TapperXP, (int val) => config.TapperXP = val,
                    name: () => "Tapper experience gain", tooltip: () => "Amount of experience gained for harvesting from tappers.\nMod default is 10, vanilla is 0.");
            api.AddBoolOption(manifest, () => config.GathererAffectsTappers, (bool val) => config.GathererAffectsTappers = val,
                   name: () => "Gatherer perk affect tappers", tooltip: () => "The gatherer foraging perk (vanilla) gives a chance for double foraged items.");

            //Production times for normal tappers
            api.AddSectionTitle(manifest, text: () => "Tappers",
                tooltip: () => "These options affect production time of tappers.");

            api.AddNumberOption(manifest, () => config.DaysForSyrups, (float val) => config.DaysForSyrups = val,
                    name: () => "Days for maple/oak/pine trees", tooltip: () => "Number of days for regular tappers to produce on listed trees.\nVanilla is 5-9 depending on tree type.");
            api.AddNumberOption(manifest, () => config.DaysForSap, (float val) => config.DaysForSap = val,
                    name: () => "Days for mahogany trees", tooltip: () => "Number of days for regular tappers to produce on mahogany trees.\nVanilla is 1.");
            api.AddNumberOption(manifest, () => config.DaysForMushroom, (float val) => config.DaysForMushroom = val,
                    name: () => "Days for mushroom trees", tooltip: () => "Number of days for regular tappers to produce on mushroom trees.\nVanilla is varies heavily based on season.\nNote that rules for *which* mushroom is produced are not changed in any way.");

            //Production time for heavy tappers
            api.AddSectionTitle(manifest, text: () => "Heavy Tappers",
                tooltip: () => "These options affect production time of heavy tappers.");

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

            //How to determine tapper product quality
            api.AddSectionTitle(manifest, text: () => "Tapper Product Quality", 
                    tooltip: () => "These options effect how output quality is determined.\nNone of these do anything if Enable quality is false (under General).");

            api.AddBoolOption(manifest, () => config.UseVanillaBotanist, (bool val) => config.UseVanillaBotanist = val,
                   name: () => "Gatherer perk affect tappers", tooltip: () => "The gatherer foraging perk (vanilla) gives a chance for double foraged items.");
            api.AddBoolOption(manifest, () => config.BotanistAffectsQuality, (bool val) => config.BotanistAffectsQuality = val,
                               name: () => "Gatherer perk affect tappers", tooltip: () => "The gatherer foraging perk (vanilla) gives a chance for double foraged items.");
            api.AddBoolOption(manifest, () => config.ForageLevelAffectsQuality, (bool val) => config.ForageLevelAffectsQuality = val,
                               name: () => "Gatherer perk affect tappers", tooltip: () => "The gatherer foraging perk (vanilla) gives a chance for double foraged items.");
            api.AddBoolOption(manifest, () => config.TimesHarvestedAffectsQuality, (bool val) => config.TimesHarvestedAffectsQuality = val,
                               name: () => "Gatherer perk affect tappers", tooltip: () => "The gatherer foraging perk (vanilla) gives a chance for double foraged items.");
            api.AddBoolOption(manifest, () => config.TreeAgeAffectsQuality, (bool val) => config.TreeAgeAffectsQuality = val,
                               name: () => "Gatherer perk affect tappers", tooltip: () => "The gatherer foraging perk (vanilla) gives a chance for double foraged items.");
        }
    }//END class
}//END namespace
