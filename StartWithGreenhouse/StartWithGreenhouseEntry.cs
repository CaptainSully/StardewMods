namespace StartWithGreenhouse
{
    using StardewModdingAPI;
    using StardewValley;
    using StardewModdingAPI.Events;

    public class StartWithGreenhouseEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            this.Helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
        }

        private static void SetPantryFlag()
        {
                Game1.player.mailReceived.Add("ccPantry");
        }
        
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (!Game1.player.mailReceived.Contains("ccPantry"))
                SetPantryFlag();
        }
    }
}