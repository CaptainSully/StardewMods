﻿using System;
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
        public static void PatchAll()
        {
            var harmony = new Harmony(ModEntry.UID);

            try
            {
                Log.T(typeof(Patcher).GetMethods().Take(typeof(Patcher).GetMethods().Length - 4).Select(mi => mi.Name)
                .Aggregate("Applying Harmony patches:", (str, s) => $"{str}{Environment.NewLine}{s}"));

                harmony.Patch(
                   original: AccessTools.Method(typeof(StardewObject), "placementAction"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchTapperPlacementAction))
                );
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "UpdateTapperProduct"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(PatchUpdateTapperProduct))
                );
            }
            catch (Exception e)
            {
                Log.E("Error while trying to setup required patches:", e);
            }
            Log.T("Patches applied successfully.");
        }

        /**
         * From StardewObject : public virtual bool placementAction(GameLocation location, int x, int y, Farmer who = null)
         */
        public static bool PatchTapperPlacementAction(ref StardewObject __instance, ref bool __result, ref GameLocation location, ref int x, ref int y, Farmer who = null)
        {
            try
            {
                Vector2 placementTile = new(x / 64, y / 64);
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
                            if (tree.growthStage.Value >= 5 && !tree.stump.Value && !location.objects.ContainsKey(placementTile))
                            {
                                Tapper tapper_instance = new(__instance.TileLocation, __instance.ParentSheetIndex);
                                tapper_instance.CopyObjTapper(__instance);
                                tapper_instance.heldObject.Value = null;
                                tapper_instance.TileLocation = placementTile;
                                location.objects.Add(placementTile, tapper_instance);
                                tree.tapped.Value = true;
                                tree.UpdateTapperProduct(tapper_instance);
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
            catch (Exception e)
            {
                Log.E("There was an exception in PatchTapperPlacementAction", e);
                return true;
            }
        }

        /**
         * From the Tree object: public void UpdateTapperProduct(Object tapper_instance, Object previous_object = null)
         */
        public static void PatchUpdateTapperProduct(ref Tree __instance, StardewObject tapper_instance, StardewObject previous_object)
        {
            try
            {
                if (previous_object != null)
                {
                    //Exp could go here. Maybe quality.
                }

                //Once product has been updated by the game, recalculate and apply a time based on mod configs
                int i = Tapper.CalculateTapperMinutes(__instance.treeType.Value, tapper_instance.ParentSheetIndex);
                if (i > 0) {
                    tapper_instance.MinutesUntilReady = i;
                }
            }
            catch (Exception e)
            {
                Log.E("There was an exception in PatchUpdateTapperProduct", e);
            }
        }
    }
}
