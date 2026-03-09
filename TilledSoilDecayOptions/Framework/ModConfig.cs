
namespace TilledSoilDecayOptions
{
    public class ModConfig
    {
        private static readonly Log log = ModEntry.Instance.log;

        /*********
        ** Accessors
        *********/
        public float DecayChance { get; set; } = 0.1f;
        public int DaysToDecay { get; set; } = 1;
        public bool Debug = false;


        /*********
        ** Public methods
        *********/
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
            api.AddNumberOption(manifest, () => config.DaysToDecay, (int val) => config.DaysToDecay = val,
                    name: () => "Days to decay",
                    tooltip: () => "Number of days the dirt must be unwatered before 'Decay chance' applies.",
                    0, 50, 1);
            api.AddNumberOption(manifest, () => config.DecayChance, (float val) => config.DecayChance = val,
                    name: () => "Decay chance",
                    tooltip: () => "Chance from 0 to 1 that tilled soil will decay overnight, if 'Days to decay' has been reached.",
                    0f, 1f, 0.01f);
            api.AddBoolOption(manifest, () => config.Debug, (bool val) => config.Debug = val,
                name: () => "Debug",
                tooltip: () => "This be VERY log spammy if there is a lot of tilled soil.");
        }
    }
}
