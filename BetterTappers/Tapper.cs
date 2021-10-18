namespace BetterTappers
{
	using Microsoft.Xna.Framework;
	using StardewValley;
	using StardewValley.TerrainFeatures;
	using StardewObject = StardewValley.Object;

	class Tapper : StardewObject
	{
		//number of times this tapper has been harvested
		public int TimesHarvested { get; set; } = 0;

		public Tapper() : base() { }

		public Tapper(Vector2 tileLocation, int parentSheetIndex, bool isRecipe = false)
			: base(tileLocation, parentSheetIndex, isRecipe) { }

        public void CopyObjTapper(StardewObject parent)
		{
			name = parent.name;
			DisplayName = parent.DisplayName;
			SpecialVariable = parent.SpecialVariable;
			_GetOneFrom(parent);
		}

		public override Item getOne()
        {
			Tapper @tapper = new Tapper(tileLocation, ParentSheetIndex);
			@tapper.name = name;
			@tapper.DisplayName = DisplayName;
			@tapper.SpecialVariable = SpecialVariable;
			@tapper._GetOneFrom(this);
			return @tapper;
		}

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

			if ((bool)readyForHarvest)
			{
				if (who.isMoving())
				{
					Game1.haltAfterCheck = false;
				}
				bool check_for_reload = false;

				if (who.IsLocalPlayer)
				{
					heldObject.Value = null;
					if (!who.addItemToInventoryBool(objectThatWasHeld))
					{
						heldObject.Value = objectThatWasHeld;
						Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
						return false;
					}
					Game1.playSound("coin");
					check_for_reload = true;
					TimesHarvested++;
				}

				if (who.currentLocation.terrainFeatures.ContainsKey(tileLocation) && who.currentLocation.terrainFeatures[tileLocation] is Tree)
				{
					(who.currentLocation.terrainFeatures[tileLocation] as Tree).UpdateTapperProduct(this, objectThatWasHeld);
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

		public override int maximumStackSize()
		{
			return 999;
		}

		//public override bool canBePlacedHere(GameLocation l, Vector2 tile) { }
	}
}
