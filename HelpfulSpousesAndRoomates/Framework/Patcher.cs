using HarmonyLib;

namespace HelpfulSpousesAndRoomates
{
    internal class Patcher
    {
        private static readonly Log log = ModEntry.Instance.log;
        private static ModConfig Config { get; set; } = ModEntry.Config;

        public static void PatchAll()
        {
            var harmony = new Harmony(ModEntry.UID);

            try
            {
                log.T(typeof(Patcher).GetMethods().Take(typeof(Patcher).GetMethods().Length - 4).Select(mi => mi.Name)
                .Aggregate("Applying Harmony patches:", (str, s) => $"{str}{Environment.NewLine}{s}"));

                // Prefix patches
                harmony.Patch(
                   original: AccessTools.Method(typeof(NPC), "marriageDuties"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_MarriageDuties))
                );

                // Postfix patches
                harmony.Patch(
                   original: AccessTools.Method(typeof(NPC), "marriageDuties"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Postfix_MarriageDuties))
                );
            }
            catch (Exception e)
            {
                log.E("Error while trying to setup required patches:", e);
            }
            log.T("Patches applied successfully.");
        }


        /*********
        ** Prefix Patches
        *********/

        public static bool Prefix_MarriageDuties()
        {
            log.T($"Modifying marriage duties.");

            if (Config.DisableMod)
            {
                log.T("Mod disabled, skipping.");
                return true;
            }

            NPC.hasSomeoneFedThePet = true;
            NPC.hasSomeoneWateredCrops = true;
            NPC.hasSomeoneFedTheAnimals = true;
            NPC.hasSomeoneRepairedTheFences = true;

            return true;
        }


        /*********
        ** Postfix Patches
        *********/

        public static void Postfix_MarriageDuties(NPC __instance)
        {
            if (Config.DisableMod)
            {
                return;
            }
            Chores.MakeChoresList(__instance);
        }
    }
}
