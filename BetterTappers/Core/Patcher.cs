using System;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewObject = StardewValley.Object;

namespace BetterTappers
{
    internal class Patcher
    {
        public static void PatchAll(ModEntry mod)
        {
            var harmony = new Harmony(ModEntry.UID);

            try
            {
                Log.T(typeof(Patcher).GetMethods().Take(typeof(Patcher).GetMethods().Count() - 4).Select(mi => mi.Name)
                .Aggregate("Applying Harmony patches:", (str, s) => $"{str}{Environment.NewLine}{s}"));

                harmony.Patch(
                   original: AccessTools.Method(typeof(StardewObject), "placementAction"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchTapperPlacementAction))
                );
            }
            catch (Exception e)
            {
                Log.E("Error while trying to setup required patches:", e);
            }
            Log.T("Patches applied successfully.");
        }

        public static bool PatchTapperPlacementAction(ref StardewObject __instance, ref bool __result, ref GameLocation location, ref int x, ref int y, Farmer who = null)
        {
            Vector2 placementTile = new Vector2(x / 64, y / 64);
            if (who != null)
            {
                __instance.owner.Value = who.UniqueMultiplayerID;
            }
            else
            {
                __instance.owner.Value = Game1.player.UniqueMultiplayerID;
            }

            switch (__instance.ParentSheetIndex)
            {
                case 105:
                case 264:
                    if (location.terrainFeatures.ContainsKey(placementTile) && location.terrainFeatures[placementTile] is Tree)
                    {
                        Tree tree = location.terrainFeatures[placementTile] as Tree;
                        if ((int)tree.growthStage >= 5 && !tree.stump && !location.objects.ContainsKey(placementTile))
                        {
                            Tapper tapper_instance = new Tapper(__instance.tileLocation, __instance.parentSheetIndex);
                            //tapper_instance.Config = ModEntry.Config;
                            tapper_instance.CopyObjTapper(__instance);
                            tapper_instance.heldObject.Value = null;
                            tapper_instance.tileLocation.Value = placementTile;
                            location.objects.Add(placementTile, tapper_instance);
                            tree.tapped.Value = true;
                            tree.UpdateTapperProduct(tapper_instance);
                            tapper_instance.SetTapperMinutes(tree.treeType);
                            location.playSound("axe");

                            __result = true;
                            return false;
                        }
                    }
                    __result = false;
                    return false;
            }
            return true;
        }
    }
}
