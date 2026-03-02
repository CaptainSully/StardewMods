
namespace HelpfulSpousesAndRoomates
{
    internal class ModConfig
    {
        private static readonly Log log = ModEntry.Instance.log;

        /*********
        ** Accessors
        *********/
        public bool DisableMod { get; set; } = false;
        public bool OnlyOneDoesChores { get; set; } = true;
        public bool RoommatesDoChores { get; set; } = true;
        public bool EnableCustomChores { get; set; } = true;
        public bool CombinePetChores { get; set; } = false;
        public bool CombineAnimalChores { get; set; } = false;
        public string GreenRainBehaviour { get; set; } = I18n.GRB_OnlyInside(); // "Only Inside", "Normal Rain", "None"
        public bool AlwaysFeedPet { get; set; } = true;
        public bool AlwaysPetPets { get; set; } = true;

        //public int MinChoresPerDay { get; set; } = 1; ???
        public int MaxChoresPerSpouse { get; set; } = 1;
        public int MaxChoresPerDay { get; set; } = Enum.GetNames(typeof(Chores.ChoreType)).Length;

        // Vanilla chores
        public float ChanceFeedPet { get; set; } = 0.20f;
        public float ChanceWaterCrops { get; set; } = 0.20f;
        public float ChanceFeedAnimals { get; set; } = 0.20f;
        public float ChanceRepairFences { get; set; } = 0.20f;
        public float ChanceMakeBreakfast { get; set; } = 0.20f;

        // Custom Chores
        public float ChancePetPet { get; set; } = 0.20f;
        public float ChancePetAnimals { get; set; } = 0.20f;
        public float ChanceWaterSlimes { get; set; } = 0.20f;

        // Debug mode
        public bool Debug { get; set; } = false;
        public bool DebugMethods { get; set; } = false;
        public bool DebugLogic { get; set; } = false;
        public bool DebugPatcher { get; set; } = false;


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

            // General settings. Some of these affect the other categories
            api.AddSectionTitle(manifest, text: () => I18n.Title_GeneralSettings());
            api.AddBoolOption(manifest, () => config.DisableMod, (bool val) => config.DisableMod = val,
                    name: () => I18n.Disabled_Name(),
                    tooltip: () => I18n.Disabled_Desc());
            api.AddBoolOption(manifest, () => config.RoommatesDoChores, (bool val) => config.RoommatesDoChores = val,
                    name: () => I18n.RoommateChores_Name(),
                    tooltip: () => I18n.RoommateChores_Desc());
            api.AddBoolOption(manifest, () => config.EnableCustomChores, (bool val) => config.EnableCustomChores = val,
                    name: () => I18n.EnableCustomChores_Name(),
                    tooltip: () => I18n.Enablecustomchores_Desc());
            api.AddBoolOption(manifest, () => config.CombinePetChores, (bool val) => config.CombinePetChores = val,
                    name: () => I18n.CombinePetChores_Name(),
                    tooltip: () => I18n.CombinePetChores_Desc(ecn: I18n.EnableCustomChores_Name(), cppn: I18n.ChancePetPets_Name()));
            api.AddBoolOption(manifest, () => config.CombineAnimalChores, (bool val) => config.CombineAnimalChores = val,
                    name: () => I18n.CombineAnimalChores_Name(),
                    tooltip: () => I18n.CombineAnimalChores_Desc(ecn: I18n.EnableCustomChores_Name(), cpan: I18n.ChancePetAnimals_Name()));
            api.AddBoolOption(manifest, () => config.AlwaysFeedPet, (bool val) => config.AlwaysFeedPet = val,
                    name: () => I18n.AlwaysFeedPets_Name(),
                    tooltip: () => I18n.AlwaysFeedPets_Desc());
            api.AddBoolOption(manifest, () => config.AlwaysPetPets, (bool val) => config.AlwaysPetPets = val,
                    name: () => I18n.AlwaysPetPets_Name(),
                    tooltip: () => I18n.AlwaysPetPets_Desc());
            api.AddTextOption(manifest, () => config.GreenRainBehaviour, (string val) => config.GreenRainBehaviour = val,
                    name: () => I18n.GreenRainBehaviour_Name(),
                    allowedValues: [I18n.GRB_OnlyInside(), I18n.GRB_NormalRain(), I18n.GRB_None()],
                    tooltip: () => I18n.GreenRainBehaviour_Desc(grboi: I18n.GRB_OnlyInside(), grbnr: I18n.GRB_NormalRain(), grbn: I18n.GRB_None()));
            api.AddNumberOption(manifest, () => config.MaxChoresPerSpouse, (int val) => config.MaxChoresPerSpouse = val,
                    name: () => I18n.MaxChoresPerSpouse_Name(),
                    tooltip: () => I18n.MaxChoresPerSpouse_Desc(),
                    1, Enum.GetNames(typeof(Chores.ChoreType)).Length, 1);

            //  Multi-spouse relevant
            api.AddSectionTitle(manifest, text: () => I18n.Title_MultiSpouseOptions());
            api.AddBoolOption(manifest, () => config.OnlyOneDoesChores, (bool val) => config.OnlyOneDoesChores = val,
                    name: () => I18n.OnlyOneDoesChores_Name(),
                    tooltip: () => I18n.OnlyOneDoesChores_Desc());
            api.AddNumberOption(manifest, () => config.MaxChoresPerDay, (int val) => config.MaxChoresPerDay = val,
                    name: () => I18n.MaxChoresPerDay_Name(),
                    tooltip: () => I18n.MaxChoresPerSpouse_Desc(),
                    1, Enum.GetNames(typeof(Chores.ChoreType)).Length, 1);

            // Vanilla chores
            api.AddSectionTitle(manifest, text: () => I18n.Title_VanillaChores());
            api.AddNumberOption(manifest, () => config.ChanceFeedPet, (float val) => config.ChanceFeedPet = val,
                    name: () => I18n.ChanceFeedPet_Name(),
                    tooltip: () => I18n.ChanceFeedPet_Desc(afpn: I18n.AlwaysFeedPets_Name()),
                    0f, 1f, 0.025f);
            api.AddNumberOption(manifest, () => config.ChanceWaterCrops, (float val) => config.ChanceWaterCrops = val,
                    name: () => I18n.ChanceWaterCrops_Name(),
                    tooltip: () => I18n.ChanceWaterCrops_Desc(),
                    0f, 1f, 0.025f);
            api.AddNumberOption(manifest, () => config.ChanceFeedAnimals, (float val) => config.ChanceFeedAnimals = val,
                    name: () => I18n.ChanceFeedAnimals_Name(),
                    tooltip: () => I18n.ChanceFeedAnimals_Desc(),
                    0f, 1f, 0.025f);
            api.AddNumberOption(manifest, () => config.ChanceRepairFences, (float val) => config.ChanceRepairFences = val,
                    name: () => I18n.ChanceRepairFences_Name(),
                    tooltip: () => I18n.ChanceRepairFences_Desc(),
                    0f, 1f, 0.025f);
            /*
            api.AddNumberOption(manifest, () => config.ChanceMakeBreakfast, (float val) => config.ChanceMakeBreakfast = val,
                    name: () => I18n.ChanceMakeBreakfast_Name(),
                    tooltip: () => I18n.ChanceMakeBreakfast_Desc(),
                    0f, 1f, 0.025f);
            */

            // Custom chores
            api.AddSectionTitle(manifest, text: () => I18n.Title_CustomChores());
            api.AddNumberOption(manifest, () => config.ChancePetPet, (float val) => config.ChancePetPet = val,
                    name: () => I18n.ChancePetPets_Name(),
                    tooltip: () => I18n.ChancePetPets_Desc(appn: I18n.AlwaysPetPets_Name(), cpcn: I18n.CombinePetChores_Name()),
                    0f, 1f, 0.025f);
            api.AddNumberOption(manifest, () => config.ChancePetAnimals, (float val) => config.ChancePetAnimals = val,
                    name: () => I18n.ChancePetAnimals_Name(),
                    tooltip: () => I18n.ChancePetAnimals_Desc(cacn: I18n.CombineAnimalChores_Name()),
                    0f, 1f, 0.025f);


            // Debuging
            api.AddParagraph(manifest, text: () => " ");
            api.AddSectionTitle(manifest, text: () => I18n.Title_Debugging());
            api.AddBoolOption(manifest, () => config.Debug, (bool val) => config.Debug = val,
                    name: () => I18n.DebugMode_Name(),
                    tooltip: () => I18n.DebugMode_Desc());
        }
    }
}
