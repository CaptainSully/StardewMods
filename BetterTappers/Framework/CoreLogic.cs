using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;

namespace BetterTappers
{
    /*********
    ** Handy Reference List:
    *   Items and IDs
    *       Tapper          - "(BC)105"
    *       Heavy Tapper    - "(BC)264"
    *       
    *       Oak Resin       - "(O)725"
    *       Maple Syrup     - "(O)724"
    *       Pine Tar        - "(O)726"
    *       Palm Oil        - "(O)247"
    *       Brown Mushroom  - "(O)404"
    *       Red Mushroom    - "(O)420"
    *       Purple Mushroom - "(O)422"
    *       Sap             - "(O)92"
    *       Moss            - "(O)Moss"
    *       Ferns           - "(O)259"
    *       Mystic Syrup    - "(O)MysticSyrup"
    *       
    *       Coconut             - "(O)88"
    *       Mushroom Tree Seed  - "(O)891"
    *       
    *   Tree Types 
    *       Oak             - "1"
    *       Maple           - "2"
    *       Pine            - "3"
    *       Palm            - "6"
    *       Mushroom        - "7"
    *       Mahogany        - "8"
    *       Palm2           - "9"
    *       GreenRain1      - "10"
    *       GreenRain2      - "11"
    *       GreenRain3      - "12"
    *       Mystic          - "13"
    *********/


    internal class CoreLogic
    {
        private static readonly Log log = ModEntry.Instance.log;
        private static readonly ModConfig Config = ModEntry.Config;

        internal static void AdjustTapperOutput(WildTreeData data, int minStack, int days, bool allYear=false)
        {
            foreach (WildTreeTapItemData tap in data.TapItems)
            {
                if (tap.ItemId == "(O)404" || tap.ItemId == "(O)420" || tap.ItemId == "(O)259")
                {
                    if (allYear)
                    {
                        tap.Condition = "";
                    }
                    else
                    {
                        tap.Condition = "!LOCATION_SEASON Target winter";
                    }
                }
                if (tap.ItemId == "(O)422")
                {
                    if (allYear)
                    {
                        tap.Condition = "DAY_OF_MONTH 10 20";
                    }
                    else
                    {
                        tap.Condition = "!LOCATION_SEASON Target Winter, DAY_OF_MONTH 10 20";
                    }
                }
                tap.MinStack = minStack;
                tap.MaxStack = minStack;
                tap.DaysUntilReady = days;
            }
        }

        /// <summary>Return number of minutes the tapper should take to produce.</summary>
        /*
		internal static int CalculateTapperMinutes(String treeType, String qualifiedItemId)
        {
            if (Config.DisableAllModEffects || !Config.ChangeTapperTimes)
            {
                return 0;
            }
            log.D("Calculating modded tapper minutes...", Config.DebugMode);

            float days_configured = 1f;
            float time_multiplier = 1f;
            int result;

            if (qualifiedItemId == "(O)264")
            {
                time_multiplier = Config.HeavyTapperMultiplier;
            }
            log.D("Time multiplier: " + time_multiplier, Config.DebugMode);

            switch (treeType)
            {
                //Oak
                case "1":
                    days_configured = Config.DaysForResin;
                    break;
                //Maple
                case "2":
                    days_configured = Config.DaysForSyrup;
                    break;
                //Pine
                case "3":
                    days_configured = Config.DaysForTar;
                    break;
                //Palm
                case "6":
                    days_configured = Config.DaysForOil;
                    break;
                //Mushroom
                case "7":
                    days_configured = Config.DaysForMushroom;
                    break;
                //Mahogany
                case "8":
                    days_configured = Config.DaysForSap;
                    break;
                //Green rain 1 (bushy)
                case "10":
                //Green rain 2 (leafy)
                case "11":
                    days_configured = Config.DaysForMoss;
                    break;
                //Green rain 3 (fern)
                case "12":
                    days_configured = Config.DaysForFern;
                    break;
                //Mystic
                case "13":
                    days_configured = Config.DaysForMystic;
                    break;

                //Modded trees
                case "FlashShifter.StardewValleyExpandedCP_Birch_Tree":
                    //days_configured = "FlashShifter.StardewValleyExpandedCP_Birch_Water";
                    break;
                case "FlashShifter.StardewValleyExpandedCP_Fir_Tree":
                    //days_configured = "FlashShifter.StardewValleyExpandedCP_Fir_Wax";
                    break;
            }

            days_configured *= time_multiplier;
            log.D("Days calculated: " + days_configured, Config.DebugMode);
            if (days_configured < 1)
            {
                result = (int)MathHelper.Max(1440 * days_configured, 5);
            }
            else
            {
                result = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int)Math.Max(1f, days_configured));
            }
            log.D("Changing minutes until ready as per configs: " + result, Config.DebugMode);
            return result;
        }
        */

        public static int GetQualityLevel(Farmer who, Tree tree)
        {
            if (Config.QualitySetting is not "None" && who is not null && tree is not null)
            {
                if (Config.BotanistAffectsTappers && who.professions.Contains(Farmer.botanist))
                {
                    log.D("Botanist perk applied", Config.DebugMode);
                    return SObject.bestQuality;
                }
                else if (Config.QualitySetting == "Tree Age")
                {
                    return QualityByTreeAge(who, GetTreeAge(tree));
                }
                else if (Config.QualitySetting == "Foraging Level")
                {
                    return QualityByForagingLevel(who);
                }
            }
            return SObject.lowQuality;
        }

        public static int QualityByForagingLevel(Farmer who)
        {
            double r = Game1.random.NextDouble();
            if (r < who.ForagingLevel / 30f)
            {
                return SObject.highQuality;
            }
            else if (r < who.ForagingLevel / 15f)
            {
                return SObject.medQuality;
            }
            else
            {
                return SObject.lowQuality;
            }
        }

        public static int QualityByTreeAge(Farmer who, int age)
        {
            if (age >= Config.TreeAgeIncrement * 2)
            {
                return SObject.highQuality;
            }
            else if (age >= Config.TreeAgeIncrement)
            {
                return SObject.medQuality;
            }
            else
            {
                return SObject.lowQuality;
            }
        }

        /// <summary>Return whether to double a stack based on gatherer perk.</summary>
		public static bool TriggerGathererPerk(Farmer who)
        {
            if (Config.GathererAffectsTappers && who.professions.Contains(Farmer.gatherer) && Game1.random.NextDouble() < 0.2)
            {
                log.D("Gatherer perk applied", Config.DebugMode);
                return true;
            }
            return false;
        }

        public static int GetTimesHarvested(SObject tapper)
        {
            tapper.modData.TryGetValue($"{ModEntry.UID}/timesHarvested", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                return int.Parse(data);
            }
            log.D("Could not get times harvested.", true);
            return 0;
        }

        internal static void IncreaseTimesHarvested(SObject tapper)
        {
            tapper.modData.TryGetValue($"{ModEntry.UID}/timesHarvested", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                tapper.modData[$"{ModEntry.UID}/timesHarvested"] = (int.Parse(data) + 1).ToString();
            }
            else
            {
                SetTimesHarvested(tapper, 0);
            }
        }

        internal static void SetTimesHarvested(SObject tapper, int num)
        {
            tapper.modData[$"{ModEntry.UID}/timesHarvested"] = num.ToString();
        }

        internal static long GetTmpUMID(SObject tapper)
        {
            tapper.modData.TryGetValue($"{ModEntry.UID}/tmpUMID", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                return long.Parse(data);
            }
            log.D("Could not get tmpUMID.", true);
            return -1;
        }

        internal static void SetTmpUMID(SObject tapper, long UMID)
        {
            tapper.modData[$"{ModEntry.UID}/tmpUMID"] = UMID.ToString();
        }

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
                SetTreeAge(tree, 1);
            }
        }

        internal static void SetTreeAge(Tree tree, int num)
        {
            tree.modData[$"{ModEntry.UID}/treeAge"] = num.ToString();
        }

        public static bool IsAnyTapper(SObject o)
        {
            return o is not null && (IsTapper(o) || IsHeavyTapper(o));
        }

        public static bool IsTapper(SObject o)
        {
            return o is not null && o.QualifiedItemId == "(BC)105";
        }

        public static bool IsHeavyTapper(SObject o)
        {
            return o is not null && o.QualifiedItemId == "(BC)264";
        }
    }
}