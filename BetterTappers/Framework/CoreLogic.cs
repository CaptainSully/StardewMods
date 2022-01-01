using StardewValley.TerrainFeatures;

namespace BetterTappers
{
    internal class CoreLogic
    {
        private static readonly Log log = ModEntry.Instance.log;
        public const int LvlCap = 100;
        public const int formula = 0;

        public static int GetTreeAgeMonths(Tree tree)
        {
            return (int)Math.Floor(GetTreeAge(tree)/28f);
        }

        public static int GetTreeAge(Tree tree)
        {
            tree.modData.TryGetValue($"{ModEntry.UID}/treeAge", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                return int.Parse(data);
            }
            log.D("Could not get tree age.", true);
            return 0;
        }

        internal static void IncreaseTreeAges()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            log.T("Increasing the age of trees.");

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

        internal static void IncreaseTreeAge(Tree tree)
        {
            tree.modData.TryGetValue($"{ModEntry.UID}/treeAge", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                tree.modData[$"{ModEntry.UID}/treeAge"] = (int.Parse(data) + 1).ToString();
            }
            else
            {
                tree.modData[$"{ModEntry.UID}/treeAge"] = "1";
            }
        }

        public static bool IsAnyTapper(SObject o)
        {
            return o is not null && o.bigCraftable.Value && (o.ParentSheetIndex == 105 || o.ParentSheetIndex == 264);
        }
        public static bool IsTapper(SObject o)
        {
            return o is not null && o.bigCraftable.Value && o.ParentSheetIndex == 105;
        }
        public static bool IsHeavyTapper(SObject o)
        {
            return o is not null && o.bigCraftable.Value && o.ParentSheetIndex == 264;
        }
    }
}