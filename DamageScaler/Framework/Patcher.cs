using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;

namespace DamageScaler.Patches
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

                harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.damageMonster), new Type[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(Farmer), typeof(bool) }),
                    prefix: new HarmonyMethod(typeof(Patcher), nameof(Before_DamageMonster))
                );
            }
            catch (Exception e)
            {
                log.E("Error while trying to setup required patches:", e);
            }
            log.T("Patches applied successfully.");
        }

        /// <summary>The method to call before <see cref="GameLocation.damageMonster(Rectangle, int, int, bool, Farmer, bool=false)"/>.</summary>
        private static void Before_DamageMonster(ref int minDamage, ref int maxDamage, Farmer who)
        {
            if (who == null)
                return;

            float scale = 1.0f + who.CombatLevel * Config.PercentDamagePerLevel * Config.PercentDamageBonus;
            int flat = who.CombatLevel * Config.FlatDamagePerLevel + Config.FlatDamageBonus;
            minDamage = Math.Max(0, (int)(minDamage * scale + flat));
            maxDamage = Math.Max(0, (int)(maxDamage * scale + flat));
        }
    }
}
