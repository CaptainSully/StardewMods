
namespace InfiniteWateringCan
{
    // <summary>The raw mod configuration.</summary>
    internal class ModConfig
    {
        private static readonly Log log = ModEntry.Instance.log;
        /// <summary>Whether the mod is disabled.</summary>
        public bool InfiniteWater { get; set; } = true;

        /// <summary>Set up Generic Mod Config Menu integration.</summary>
        public static void SetUpModConfigMenu(ModConfig config, Mod mod)
        {
            log.T("Set up GMCM.");
            // Get the Generic Mod Config Menu API
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null) { return; }
            var manifest = mod.ModManifest;

            // Register the Generic Mod Config Menu API
            api.Register(manifest, () => config = new ModConfig(), delegate { mod.Helper.WriteConfig(config); });

            // Options to display
            api.AddBoolOption(manifest, () => config.InfiniteWater, (bool val) => config.InfiniteWater = val,
                    name: () => "Infinite Water", tooltip: () => null);
            api.AddParagraph(manifest, text: () => "Infinite water in watering cans while true.");

        }
    }
}