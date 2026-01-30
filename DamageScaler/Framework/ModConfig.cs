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
        public float PerLevelDamageMultiplier { get; set; } = 1.05f;
        /// <summary>Flat damage boost added per combat skill level.</summary>
        public int PerLevelDamageBonus { get; set; } = 0;
        /// <summary>Non-scaling percent damage boost added.</summary>
        public float SingleDamageMultiplier { get; set; } = 1f;
        /// <summary>Non-scaling damage boost added.</summary>
        public int SingleDamageBonus { get; set; } = 0;


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

            if (config.PerLevelDamageMultiplier < 0)
            {
                invalidConfig = true;
                config.PerLevelDamageMultiplier = 1.05f;
            }

            if (config.SingleDamageMultiplier < 0)
            {
                invalidConfig = true;
                config.SingleDamageMultiplier = 1f;
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
            api.AddNumberOption(manifest, () => config.PerLevelDamageMultiplier, (float val) => config.PerLevelDamageMultiplier = val,
                    name: () => "Per level damage multiplier", tooltip: () => "Multiplies damage according to your combat skill level.\nLess than 1 decreases; greater than 1 increases damage.");
            api.AddNumberOption(manifest, () => config.PerLevelDamageBonus, (int val) => config.PerLevelDamageBonus = val,
                    name: () => "Per level damage bonus", tooltip: () => "Damage added per combat skill level.\nMod default is 0.");

            api.AddSectionTitle(manifest, text: () => "Single damage bonuses",
                tooltip: () => "These options do not scale.");
            api.AddNumberOption(manifest, () => config.SingleDamageMultiplier, (float val) => config.SingleDamageMultiplier = val,
                   name: () => "One time damage multiplier", tooltip: () => "One time multiplier to damage.\nLess than 1 decreases; greater than 1 increases damage.");
            api.AddNumberOption(manifest, () => config.SingleDamageBonus, (int val) => config.SingleDamageBonus = val,
                    name: () => "One time damage bonus", tooltip: () => "Amount of flat damage to add.\nMod default is 0.");


            // Debuging
            api.AddParagraph(manifest, text: () => " ");
            api.AddSectionTitle(manifest, text: () => "Debuging");
            api.AddBoolOption(manifest, () => config.DebugMode, (bool val) => config.DebugMode = val,
                    name: () => "Debug mode", tooltip: () => "This is for helping me debug/test things.\nEnable only if you're trying to do the same.");
        }
    }
}
