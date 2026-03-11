using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System.Reflection.Emit;

namespace TilledSoilDecayOptions
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
                   original: AccessTools.Method(typeof(HoeDirt), "dayUpdate"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_dayUpdate))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(GameLocation), "GetDirtDecayChance"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_GetDirtDecayChance))
                );

                // Transpiler patches
                harmony.Patch(
                   original: AccessTools.Method(typeof(GameLocation), "HandleGrassGrowth"),
                   transpiler: new HarmonyMethod(typeof(Patcher), nameof(Transpiler_HandleGrassGrowth))
                );
            }
            catch (Exception e)
            {
                log.E("Error while trying to setup required patches:", e);
            }
            log.T("Patches applied successfully.");
        }


        /*********
        ** Transpiler Patches
        *********/
        // Completely skips removing hoedirt on the farm from season changes
        public static IEnumerable<CodeInstruction> Transpiler_HandleGrassGrowth(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            // If "is Farm", then skip to the next code block, thus not doing any HoeDirt removals on farms.
            // If someone added "ClearEmptyDirtOnNewMonth" to Ginger Island farm it won't work there.
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Brfalse_S));
            Label lbl = (Label)matcher.Operand;

            matcher.Start().MatchStartForward(
                new CodeMatch(OpCodes.Brtrue_S));
            matcher.SetOperandAndAdvance(lbl);
            
            // Alternative solution
            // Remove the "is Farm" check, so only the property "ClearEmptyDirtOnNewMonth" is checked.
            // Farms don't have this by default, so no seasonal decay unless added. Same with Ginger Island farm.
            /*
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Isinst),
                new CodeMatch(OpCodes.Brtrue_S)
                );
            matcher.RemoveInstructions(3);
            */

            return matcher.InstructionEnumeration();
        }


        /*********
        ** Prefix Patches
        *********/

        // Runs before GetDirtDecayChance. If dirt is watered then reset timer, otherwise reduce timer.
        public static bool Prefix_dayUpdate(HoeDirt __instance)
        {
            if (__instance.Location is Farm || __instance.Location is IslandWest || __instance.Location.isFarm.Value)
            {
                if (__instance.state.Value == 1)
                {
                    SetDelay(__instance, Config.DaysToDecay);
                }
                else
                {
                    SetDelay(__instance, GetDelay(__instance) - 1);
                }
                log.D($"HoeDirt.dayUpdate; watered: {__instance.state.Value}, delay: {GetDelay(__instance)}", Config.Debug);
            }
            return true;
        }

        // Return 0 decay chance unless the timer is 0 or less
        public static bool Prefix_GetDirtDecayChance(GameLocation __instance, ref double __result, Vector2 tile)
        {
            if (__instance is Farm || __instance is IslandWest || __instance.isFarm.Value)
            {
                HoeDirt dirt = __instance.GetHoeDirtAtTile(tile);
                if (dirt is null) { return true; }
                if (GetDelay(dirt) <= 0)
                {
                    __result = (double)Config.DecayChance;
                    log.D($"GetDirtDecayChance result: {__result}", Config.Debug);
                    return false;
                }
                else
                {
                    __result = 0.0;
                    log.D($"GetDirtDecayChance result: {__result}", Config.Debug);
                    return false;
                }
            }
            return true;
        }

        /*********
        ** Helper methods
        *********/
        internal static int GetDelay(HoeDirt hoeDirt)
        {
            hoeDirt.modData.TryGetValue($"{ModEntry.UID}/currentDelay", out string data);

            // If the delay hasn't been set, or is greater than the config settings, reset it
            if (string.IsNullOrEmpty(data) || int.Parse(data) > Config.DaysToDecay)
            {
                SetDelay(hoeDirt, Config.DaysToDecay);
                return Config.DaysToDecay;
            }
            else
            {
                return int.Parse(data);
            }
        }

        internal static void SetDelay(HoeDirt hoeDirt, int delay)
        {
            hoeDirt.modData[$"{ModEntry.UID}/currentDelay"] = delay.ToString();
        }
    }
}