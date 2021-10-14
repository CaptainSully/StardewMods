namespace BetterTappers
{
    using System;
    using StardewModdingAPI;

    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);
    }
    public class BetterTappersConfig
    {
        //options for regular tappers
        public float DaysForSyrups { get; set; } = 7f;
        public float DaysForSap { get; set; } = 1f;
        public float DaysForMushroom { get; set; } = 7f;

        //options for hardwood tappers
        public Boolean OverrideHeavyTapperDefaults = false;
        public float DaysForSyrupsHeavy { get; set; } = 3.5f;
        public float DaysForSapHeavy { get; set; } = 0.5f;
        public float DaysForMushroomHeavy { get; set; } = 3.5f;

        //other options
        //public int TapperQualityOptions { get; set; } = 0;
        //public int TapperExp { get; set; } = 0;

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
            GenericModConfigMenuAPI api = mod.Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.RegisterModConfig(manifest, () => config = new BetterTappersConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            api.RegisterLabel(manifest, "Tappers", null);

            api.RegisterSimpleOption(manifest, "Days for maple/oak/pine trees", "Number of days for regular tappers to produce on listed trees.\nVanilla is 5-9 depending on tree type.", () => config.DaysForSyrups, (float val) => config.DaysForSyrups = val);
            api.RegisterSimpleOption(manifest, "Days for mahogany trees", "Number of days for regular tappers to produce on mahogany trees.\nVanilla is 1.", () => config.DaysForSap, (float val) => config.DaysForSap = val);
            api.RegisterSimpleOption(manifest, "Days for mushroom trees", "Number of days for regular tappers to produce on mushroom trees.\nVanilla is varies heavily.\nNote that rules for type of mushroom are not changed in any way.", () => config.DaysForMushroom, (float val) => config.DaysForMushroom = val);

            api.RegisterLabel(manifest, "Heavy Tappers", null);

            api.RegisterSimpleOption(manifest, "Manually set times for heavy tappers?", "Defaults to half of normal tappers if false.", () => config.OverrideHeavyTapperDefaults, (bool val) => config.OverrideHeavyTapperDefaults = val);
            api.RegisterSimpleOption(manifest, "Days for maple/oak/pine trees", "Number of days for regular tappers to produce on listed trees.", () => config.DaysForSyrupsHeavy, (float val) => config.DaysForSyrupsHeavy = val);
            api.RegisterSimpleOption(manifest, "Days for mahogany trees", "Number of days for regular tappers to produce on mahogany trees.", () => config.DaysForSapHeavy, (float val) => config.DaysForSapHeavy = val);
            api.RegisterSimpleOption(manifest, "Days for mushroom trees", "Number of days for regular tappers to produce on mushroom trees.\nVanilla is varies heavily.\nNote that rules for type of mushroom are not changed in any way.", () => config.DaysForMushroomHeavy, (float val) => config.DaysForMushroomHeavy = val);
        }
    }//END class
}//END namespace
