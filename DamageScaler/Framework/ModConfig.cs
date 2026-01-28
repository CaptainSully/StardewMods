namespace DamageScaler.Patches
{
    // <summary>The raw mod configuration.</summary>
    public class ModConfig
    {
        /// <summary>Logging tool</summary>
        private static readonly Log log = ModEntry.Instance.log;

        /*********
        ** Accessors
        *********/
        /// <summary>Whether the mod is disabled.</summary>
        public bool DisableAllModEffects { get; set; } = false;
        /// <summary>Percent damage boost added per combat skill level.</summary>
        public float PercentDamagePerLevel { get; set; } = 1.05f;
        /// <summary>Flat damage boost added per combat skill level.</summary>
        public int FlatDamagePerLevel { get; set; } = 0;
        /// <summary>Non-scaling percent damage boost added.</summary>
        public float PercentDamageBonus { get; set; } = 1f;
        /// <summary>Non-scaling damage boost added.</summary>
        public int FlatDamageBonus { get; set; } = 0;


        // Debug mode
        /// <summary>General on the fly debug mode.</summary>
        public bool DebugMode { get; set; } = false;
        /// <summary>Log method calls.</summary>
        internal bool DebugMethods { get; set; } = false;
        /// <summary>Enable more specific logging.</summary>
        internal bool DebugLogic { get; set; } = false;
        /// <summary>Log patcher logic.</summary>
        internal bool DebugPatcher { get; set; } = false;

        /*********
        ** Public methods
        *********/
        /// <summary>Check for and reset any invalid configuration settings.</summary>
        public static void VerifyConfigValues(ModConfig config, Mod mod)
        {
            bool invalidConfig = false;

            if (config.PercentDamagePerLevel < 0)
            {
                invalidConfig = true;
                config.PercentDamagePerLevel = 1.05f;
            }

            if (config.FlatDamagePerLevel < 0)
            {
                invalidConfig = true;
                config.FlatDamagePerLevel = 0;
            }
            if (config.PercentDamageBonus < 0)
            {
                invalidConfig = true;
                config.PercentDamageBonus = 1f;
            }

            if (config.FlatDamageBonus < 0)
            {
                invalidConfig = true;
                config.FlatDamageBonus = 0;
            }

            if (invalidConfig)
            {
                log.I("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        /// <summary>Set up Generic Mod Config Menu integration.</summary>
        public static void SetUpModConfigMenu(ModConfig config, Mod mod)
        {
            // Get the Generic Mod Config Menu API
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null) { return; }
            var manifest = mod.ModManifest;

            // Register the Generic Mod Config Menu API
            api.Register(manifest, () => config = new ModConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            // Options to display
            api.AddBoolOption(manifest, () => config.DisableAllModEffects, (bool val) => config.DisableAllModEffects = val,
                    name: () => "Disable this mods effects", tooltip: () => "Game will follow vanilla behaviour if true.\n'True' overrides ALL other settings.");
            
            api.AddSectionTitle(manifest, text: () => "Scaling damage bonuses",
                tooltip: () => "These options scale with your Combat Skill level.");
            api.AddNumberOption(manifest, () => config.PercentDamagePerLevel, (float val) => config.PercentDamagePerLevel = val,
                    name: () => "Percent damage bonus per level", tooltip: () => "Multiplier to damage per combat skill level.\nLess than 1 decreases; greater than 1 increases damage.");
            api.AddNumberOption(manifest, () => config.FlatDamagePerLevel, (int val) => config.FlatDamagePerLevel = val,
                    name: () => "Flat damage bonus per level", tooltip: () => "Amount of flat damage to add per combat skill level.\nMod default is 0.");

            api.AddSectionTitle(manifest, text: () => "Single damage bonuses",
                tooltip: () => "These options do not scale with anything.");
            api.AddNumberOption(manifest, () => config.PercentDamageBonus, (float val) => config.PercentDamageBonus = val,
                   name: () => "Percent damage bonus.", tooltip: () => "Multiplier to damage: 1 is no change; 1.05 is a 5% increase.\nLess than 1 decreases; greater than 1 increases damage.");
            api.AddNumberOption(manifest, () => config.FlatDamageBonus, (int val) => config.FlatDamageBonus = val,
                    name: () => "Flat damage bonus.", tooltip: () => "Amount of flat damage to add.\nMod default is 0.");


            // Debuging
            api.AddParagraph(manifest, text: () => " ");
            api.AddSectionTitle(manifest, text: () => "Debuging");
            api.AddBoolOption(manifest, () => config.DebugMode, (bool val) => config.DebugMode = val,
                    name: () => "Debug mode", tooltip: () => "This is for helping me debug/test things.\nEnable only if you're trying to do the same.");
        }
    }
}
