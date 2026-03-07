using HarmonyLib;
using Microsoft.Xna.Framework;

namespace DamageScaler
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
                    //original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.damageMonster), new Type[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(Farmer), typeof(bool) }),
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.damageMonster), new Type[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer), typeof(bool) }),
                    prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_DamageMonster))
                );
            }
            catch (Exception e)
            {
                log.E("Error while trying to setup required patches:", e);
            }
            log.T("Patches applied successfully.");
        }

        private static void Prefix_DamageMonster(ref int minDamage, ref int maxDamage, Farmer who)
        {
            if (who == null || Config.DisableAllModEffects)
                return;
            log.T($"Modifying damage to monster by {who.Name}; min: {minDamage}, max: {maxDamage}");

            float levelMultiplier = 1f;

            if (who.CombatLevel == 0)
                levelMultiplier = 0;

            else if (Config.PerLevelDamageMultiplier > 1)
                levelMultiplier = 1 + (who.CombatLevel * (Config.PerLevelDamageMultiplier - 1));

            else if (Config.PerLevelDamageMultiplier < 1)
                levelMultiplier = 1 - (who.CombatLevel * (1 - Config.PerLevelDamageMultiplier));

            float multiplier = levelMultiplier * Config.SingleDamageMultiplier;
            int flat = who.CombatLevel * Config.PerLevelDamageBonus + Config.SingleDamageBonus;

            minDamage = Math.Max(0, (int)(minDamage * multiplier + flat));
            maxDamage = Math.Max(minDamage, (int)(maxDamage * multiplier + flat));
            log.T($"New damage calc; min: {minDamage}, max: {maxDamage}");
        }
    }
}
