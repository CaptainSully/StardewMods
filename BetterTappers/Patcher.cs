namespace BetterTappers
{
    using System;
    using HarmonyLib;
    using StardewValley.TerrainFeatures;
    using StardewObject = StardewValley.Object;

    internal class Patcher
    {
        private static BetterTappersEntry mod;

        public static void PatchAll(BetterTappersEntry btaps)
        {
            mod = btaps;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "UpdateTapperProduct"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(PatchTapperTime))
                );
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        public static void PatchTapperTime(ref Tree __instance, ref StardewObject tapper_instance, ref StardewObject previous_object)
        {
            try
            {
                if (__instance != null && tapper_instance != null && BetterTappersLogic.IsAnyTapper(tapper_instance))
                { 
                    tapper_instance.MinutesUntilReady = BetterTappersLogic.DesiredMinutes(mod, tapper_instance.parentSheetIndex, __instance);
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }
    }
}
