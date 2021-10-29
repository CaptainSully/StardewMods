using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using StardewObject = StardewValley.Object;

namespace BetterTappers
{
    internal class BetterTappersLogic
    {
        public const int LvlCap = 100;
        public const int formula = 0;

        public static int GetTreeAgeMonths(Tree tree)
        {
            return (int)Math.Floor(GetTreeAge(tree)/28f);
        }

        public static int GetTreeAge(Tree tree)
        {
            tree.modData.TryGetValue($"{BetterTappersEntry.UID}/treeAge", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                return int.Parse(data);
            }
            BetterTappersEntry.DebugLog("BetterTappers: Could not get tree age.", true);
            return 0;
        }

        public static void IncreaseTreeAges()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            foreach (var location in Game1.locations)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is Tree tree)
                    {
                        IncreaseTreeAge(tree);
                    }
                }
            }
        }

        public static void IncreaseTreeAge(Tree tree)
        {
            tree.modData.TryGetValue($"{BetterTappersEntry.UID}/treeAge", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                tree.modData[$"{BetterTappersEntry.UID}/treeAge"] = (int.Parse(data) + 1).ToString();
            }
            else
            {
                tree.modData[$"{BetterTappersEntry.UID}/treeAge"] = "1";
            }
        }

        public static bool IsAnyTapper(StardewObject o)
        {
            return o != null && o.bigCraftable && (o.ParentSheetIndex == 105 || o.parentSheetIndex == 264);
        }
        public static bool IsTapper(StardewObject o)
        {
            return o != null && o.bigCraftable && o.parentSheetIndex == 105;
        }
        public static bool IsHeavyTapper(StardewObject o)
        {
            return o != null && o.bigCraftable && o.parentSheetIndex == 264;
        }
    }
}