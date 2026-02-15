using HarmonyLib;
using StardewValley.GameData.Machines;
using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;

namespace BetterTappers
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
                   original: AccessTools.Method(typeof(Tree), "IsGrowthBlockedByNearbyTree"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_IsGrowthBlockedByNearbyTree))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "GetMaxSizeHere"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_GetMaxSizeHere))
                );

                // Postfix patches
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "GetData"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Postfix_TapperAndTreeData))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(DataLoader), "Machines"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Postfix_TapperExperience))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(DataLoader), "WildTrees"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Postfix_TappableTrees))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "GetData"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Postfix_TapperAndTreeData))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "UpdateTapperProduct"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Postfix_UpdateTapperOutput))
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

        public static bool Prefix_IsGrowthBlockedByNearbyTree(Tree __instance, ref bool __result)
        {
            if (!Config.NearbyTreesBlockGrowth)
            {
                __result = false;
                return false;
            }
            return true;
        }


        public static bool Prefix_GetMaxSizeHere(Tree __instance, ref int __result)
        {
            if (!Config.NearbyTreesBlockGrowth)
            {
                __result = 15;
                return false;
            }
            return true;
        }


        /*********
        ** Postfix Patches
        *********/
        
        public static void Postfix_TapperExperience(ref Dictionary<string, MachineData> __result)
        {
            if (__result.TryGetValue("(BC)105", out MachineData tapper))
            {
                tapper.ExperienceGainOnHarvest = "Foraging " + Config.TapperXP;
            }
            if (__result.TryGetValue("(BC)264", out MachineData htapper))
            {
                htapper.ExperienceGainOnHarvest = "Foraging " + Config.TapperXP;
            }
        }


        public static void Postfix_TappableTrees(ref Dictionary<string, WildTreeData> __result)
        {
            if (__result.TryGetValue("6", out WildTreeData palm))
            {
                palm.SeedPlantable = Config.CoconutPalmSeed;
                palm.SeedItemId = "(O)88";
                palm.TapItems = new List<WildTreeTapItemData>
                {
                    new WildTreeTapItemData
                    {
                        ItemId = "(O)247",
                        MinStack = Config.AmountOfOil,
                        MaxStack = Config.AmountOfOil,
                        DaysUntilReady = Config.DaysForOil
                    }
                };
            }
            if (__result.TryGetValue("9", out WildTreeData palm2))
            {
                palm2.SeedPlantable = Config.CoconutPalmSeed;
                palm2.SeedItemId = "(O)88";
                palm2.TapItems = new List<WildTreeTapItemData>
                {
                    new WildTreeTapItemData
                    {
                        ItemId = "(O)247",
                        MinStack = Config.AmountOfOil,
                        MaxStack = Config.AmountOfOil,
                        DaysUntilReady = Config.DaysForOil
                    }
                };
            }
            if (__result.TryGetValue("10", out WildTreeData greenrain1))
            {
                greenrain1.TapItems = new List<WildTreeTapItemData>
                {
                    new WildTreeTapItemData
                    {
                        ItemId = "(O)Moss",
                        MinStack = Config.AmountOfMoss,
                        MaxStack = Config.AmountOfMoss,
                        DaysUntilReady = Config.DaysForMoss
                    }
                };
            }
            if (__result.TryGetValue("11", out WildTreeData greenrain2))
            {
                greenrain2.TapItems = new List<WildTreeTapItemData>
                {
                    new WildTreeTapItemData
                    {
                        ItemId = "(O)Moss",
                        MinStack = Config.AmountOfMoss,
                        MaxStack = Config.AmountOfMoss,
                        DaysUntilReady = Config.DaysForMoss
                    }
                };
            }
        }
        
        
        public static void Postfix_TapperAndTreeData(ref Tree __instance, ref WildTreeData __result)
        {
            if (__result == null)
            {
                return;
            }

            __result.GrowsInWinter = Config.GrowsInWinter;

            string type = __instance.treeType.Value;
            if (__result.TapItems == null)
            {
                return;
            }
            switch (type)
            {
                case "1":
                    CoreLogic.AdjustTapperOutput(__result, Config.AmountOfResin, Config.DaysForResin);
                    break;

                case "2":
                    CoreLogic.AdjustTapperOutput(__result, Config.AmountOfSyrup, Config.DaysForSyrup);
                    break;

                case "3":
                    CoreLogic.AdjustTapperOutput(__result, Config.AmountOfTar, Config.DaysForTar);
                    break;

                case "7":
                    CoreLogic.AdjustTapperOutput(__result, Config.AmountOfMushrooms, Config.DaysForMushroom, Config.ProduceInWinter);
                    __result.IsStumpDuringWinter = Config.IsStumpDuringWinter;
                    break;

                case "8":
                    CoreLogic.AdjustTapperOutput(__result, Config.AmountOfSap, Config.DaysForSap);
                    break;

                case "12":
                    CoreLogic.AdjustTapperOutput(__result, Config.AmountOfFerns, Config.DaysForFern, Config.ProduceInWinter);
                    __result.IsStumpDuringWinter = Config.IsStumpDuringWinter;
                    break;

                case "13":
                    CoreLogic.AdjustTapperOutput(__result, Config.AmountOfMysticSyrup, Config.DaysForMystic);
                    break;

                    //Palm trees 1 and 2
                case "6":
                case "9":
                    CoreLogic.AdjustTapperOutput(__result, Config.AmountOfOil, Config.DaysForOil);
                    break;

                    //Green rain trees 1 and 2
                case "10":
                case "11":
                    CoreLogic.AdjustTapperOutput(__result, Config.AmountOfMoss, Config.DaysForMoss);
                    break;
            }
        }

        
        public static void Postfix_UpdateTapperOutput(ref Tree __instance, SObject tapper, SObject previousOutput = null, bool onlyPerformRemovals = false)
        {
            if (tapper is null || tapper.heldObject is null || tapper.heldObject.Value is null || onlyPerformRemovals)
            {
                return;
            }

            Farmer lastHitBy = Game1.GetPlayer(__instance.lastPlayerToHit.Value) ?? Game1.MasterPlayer;
            if (__instance.treeType.Value != "8" && __instance.treeType.Value != "10" && __instance.treeType.Value != "11")
            {
                tapper.heldObject.Value.Quality = CoreLogic.GetQualityLevel(lastHitBy, __instance);
            }
            if (CoreLogic.TriggerGathererPerk(lastHitBy))
            {
                tapper.heldObject.Value.Stack *= 2;
            }
            CoreLogic.IncreaseTimesHarvested(tapper);
        }
    }
}
