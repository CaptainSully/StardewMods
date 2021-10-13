﻿namespace BetterTappers
{
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Objects;
    using StardewValley.TerrainFeatures;
    using StardewModdingAPI.Events;
    using StardewObject = StardewValley.Object;

    internal class BetterTappersLogic
    {
        public static void CheckTappers(ModEntry mod, Object o)
        {
            if (IsAnyTapper(o))
            {
                ChangeTapperMinutes(mod, o);
            }
        }

        public static void ChangeTapperMinutes(ModEntry mod, Object tapper)
        {
            foreach (var terrainfeature in Game1.currentLocation.terrainFeatures.Pairs)
            {
                if (terrainfeature.Value is Tree tree)
                {
                    if (tree.currentTileLocation == tapper.TileLocation)
                    {
                        tapper.MinutesUntilReady = DesiredMinutes(mod, tapper.parentSheetIndex, tree);
                    }
                }
            }
        }

        public static int DesiredMinutes(ModEntry mod, int index, Tree tree)
        {
            if (index == 105)
            {
                switch (tree.treeType)
                {
                    case 1:
                    case 2:
                    case 3:
                        return (int)(1440 * mod.Config.DaysForSyrups);
                    case 7:
                        return (int)(1440 * mod.Config.DaysForMushroom);
                    case 8:
                        return (int)(1440 * mod.Config.DaysForSap);
                    default:
                        return 1440;
                }
            }
            else if (index == 264)
            {
                if (mod.Config.OverrideHeavyTapperDefaults)
                {
                    switch (tree.treeType)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return (int)(1440 * mod.Config.DaysForSyrupsHeavy);
                        case 7:
                            return (int)(1440 * mod.Config.DaysForMushroomHeavy);
                        case 8:
                            return (int)(1440 * mod.Config.DaysForSapHeavy);
                        default:
                            return 1440;
                    }
                }
                else
                {
                    switch (tree.treeType)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return (int)((1440 * mod.Config.DaysForSyrups) / 2);
                        case 7:
                            return (int)((1440 * mod.Config.DaysForMushroom) / 2);
                        case 8:
                            return (int)((1440 * mod.Config.DaysForSap) / 2);
                        default:
                            return 1440;
                    }
                }
            }
            else
            {
                mod.Monitor.Log($"Error in: BetterTappers.DesiredMinutes method, somehow the object is a tapper but isn't a tapper.", LogLevel.Debug);
                return 1440;
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