﻿using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Xml.Serialization;
using StardewObject = StardewValley.Object;


namespace BetterTappers
{
	[XmlType("Mods_CaptainSully_BetterTappers_Tapper")] // SpaceCore serialisation signature
	public class Tapper : StardewObject
	{
		// Custom variables
		private static Config Config { get; set; } = ModEntry.Config;
		public int TimesHarvested { get; set; } = 0;


		// Overrides
        public Tapper() : base() { }

		public Tapper(int parentSheetIndex) : base()
        {
			ParentSheetIndex = parentSheetIndex; 
		}

		public Tapper(Vector2 tileLocation, int parentSheetIndex, bool isRecipe = false)
			: base(tileLocation, parentSheetIndex, isRecipe) { }

		//this is actually currently useless since tapper objects only exist outside the inventory
		/*public override int maximumStackSize()
		{
			if (Stackable) { return 999; }
			return 1;
		}*/

		//potentially useful for making new tapper types, or adding them to different trees etc.
		/*public override bool canBePlacedHere(GameLocation l, Vector2 tile)
		{

		}*/

		public override Item getOne()
        {
            Tapper @tapper = new(tileLocation, ParentSheetIndex)
            {
                name = name,
                DisplayName = DisplayName,
                SpecialVariable = SpecialVariable
            };
            @tapper._GetOneFrom(this);
			return @tapper;
		}

		// Mostly vanilla behaviour thats been stripped of things unrelated to tappers. New things have comments
		public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
		{
			if (isTemporarilyInvisible || justCheckingForActivity)
			{
				return true;
			}
			if (!justCheckingForActivity && who != null && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1) && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1) && who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY()) && who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY()) && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
			{
				performToolAction(null, who.currentLocation);
			}

			StardewObject objectThatWasHeld = heldObject.Value;

			if (readyForHarvest.Value)
			{
				if (who.isMoving())
				{
					Game1.haltAfterCheck = false;
				}
				bool check_for_reload = false;

				Tree tree = null;
				if (who.IsLocalPlayer)
				{
					heldObject.Value = null;

					//Change quality value of objectThatWasHeld, then apply gatherer perk
					if (who.currentLocation.terrainFeatures.ContainsKey(tileLocation) && who.currentLocation.terrainFeatures[tileLocation] is Tree)
					{
						tree = (who.currentLocation.terrainFeatures[tileLocation] as Tree);
						if (tree.treeType.Value != 8)
						{
							objectThatWasHeld.Quality = GetQualityLevel(who, CoreLogic.GetTreeAgeMonths(tree));
						}
						objectThatWasHeld.Stack = TriggerGathererPerk(who);
					}

					if (!who.addItemToInventoryBool(objectThatWasHeld))
					{
						//if harvesting failed, reset quality of the ready item back to low and stack size back to 1
						objectThatWasHeld.Quality = lowQuality;
						objectThatWasHeld.Stack = 1;
						heldObject.Value = objectThatWasHeld;
						Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
						return false;
					}
					Game1.playSound("coin");
					check_for_reload = true;

					//Give player experience, and then increment tapper TimesHarvested
					who.gainExperience(2, Config.TapperXP);
					TimesHarvested++;
				}

				//vanilla if statement moved up because quality needs to know if there's a tree. replaced with this check.
				if (tree != null)
				{
					tree.UpdateTapperProduct(this, objectThatWasHeld);
				}

				readyForHarvest.Value = false;
				showNextIndex.Value = false;

				if (check_for_reload)
				{
					AttemptAutoLoad(who);
				}
				return true;
			}
			return false;
		}


		// Custom methods
		public void CopyObjTapper(StardewObject parent)
		{
			name = parent.name;
			DisplayName = parent.DisplayName;
			SpecialVariable = parent.SpecialVariable;
			_GetOneFrom(parent);
		}

		private static int TriggerGathererPerk(Farmer who)
        {
			Log.D("Checking gatherer perk...", Config.DebugMode);
			if (who.professions.Contains(Farmer.gatherer) && Game1.random.NextDouble() < 0.2)
            {
				return 2;
			}
			return 1;
		}
		
		public static int CalculateTapperMinutes(int treeType, int parentSheetIndex)
		{
			if (Config.DisableAllModEffects || !Config.ChangeTapperTimes)
			{
				return 0;
			}
			Log.D("Calculating modded tapper minutes...", Config.DebugMode);

			float days_configured = 1f;
			float time_multiplier = 1f;
			int result;

			if (parentSheetIndex == 264)
			{
				time_multiplier = Config.HeavyTapperMultiplier;
				Log.D("Time multiplier: " + time_multiplier, Config.DebugMode);
			}

			switch (treeType)
			{
				case 1:
				case 2:
				case 3:
					days_configured = Config.DaysForSyrups;
					break;
				case 7:
					days_configured = Config.DaysForMushroom;
					break;
				case 8:
					days_configured = Config.DaysForSap;
					break;
			}

			days_configured *= time_multiplier;
			Log.D("Days: " + days_configured, Config.DebugMode);
			if (days_configured < 1)
			{
				result = (int)MathHelper.Max(1440 * days_configured, 5);
			}
			else
            {
				result = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int)Math.Max(1f, days_configured));
			}
			Log.D("Changing minutes until ready as per configs: " + result, Config.DebugMode);
			return result;
		}

		public int GetQualityLevel(Farmer who, int age)
        {
			Log.D("Quality check requested...", Config.DebugMode);
			if (Config.DisableAllModEffects || !Config.TappersUseQuality)
			{
				return lowQuality;
			}
			if ((!Config.ForageLevelAffectsQuality || who == null) && (!Config.TreeAgeAffectsQuality || age < 1) && !Config.TimesHarvestedAffectsQuality)
            {
				return lowQuality;
            }

			int quality = DetermineQuality(who.foragingLevel.Value, age);
			if (Config.BotanistAffectsTappers)
			{
				if (who != null && who.professions.Contains(Farmer.botanist))
				{
					Log.D("Botanist perk applied.", Config.DebugMode);
					return bestQuality;
				}
				return Math.Min(quality, highQuality);
			}
			if (quality == 3)
            {
				return highQuality;
            }
			return quality;
		}

		private int DetermineQuality(int foragingLevel, int age = 0)
		{
			Log.D("Determining quality...", Config.DebugMode);
			int n, FLQ, TAQ, THQ, t;
			n = FLQ = TAQ = THQ = 0;

			if (Config.ForageLevelAffectsQuality)
            {
				FLQ = GetQualityPart(foragingLevel);
				n++;
			}
			if (Config.TreeAgeAffectsQuality)
			{
				TAQ = GetQualityPart(age);
				n++;
			}
			if (Config.TimesHarvestedAffectsQuality)
			{
				THQ = GetQualityPart(TimesHarvested);
				n++;
			}

			Log.D("QualitiesActive: " + n + "    FLQ: " + FLQ + "    TAQ: " + TAQ + "    THQ: " + THQ, Config.DebugMode);
			t = (FLQ + TAQ + THQ);
			Log.D("Sum of qualty pieces: " + t, Config.DebugMode);
			switch (n)
            {
				case 3:
					if (t == 6)
					{
						return bestQuality;
					}
					return (int)Math.Floor(t * 0.5);

				case 2:
					return (int)Math.Floor(t * 0.75);
				case 1:
					return t;
				//these shouldn't happen, but if they do return low
				case 0:
				default:
					Log.D("Problem: shouldn't asking for quality when no quality types are enabled. Defaulted to low.", true);
					return lowQuality;
			}
		}

		private static int GetQualityPart(int lvl)
        {
			Log.D("Getting quality piece...", Config.DebugMode);
			if (lvl > 0)
			{
				double ran = Game1.random.NextDouble();
				switch (CoreLogic.formula)
				{
					case 0:
					default:
						if (ran < (Math.Min(lvl, CoreLogic.LvlCap) / 30f))
						{
							return highQuality;
						}
						else if (ran < (Math.Min(lvl, CoreLogic.LvlCap) / 15f))
						{
							return medQuality;
						}
						break;
				}
			}
			return lowQuality;
		}
	}//END class
}//END namespace
