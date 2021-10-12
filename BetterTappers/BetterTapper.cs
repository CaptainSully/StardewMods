namespace BetterTappers
{
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Objects;
    using StardewModdingAPI.Events;
    using StardewObject = StardewValley.Object;

    internal class BetterTappersLogic
    {
        public static void CheckTappers(ModEntry mod, ObjectListChangedEventArgs e)
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            foreach (var Object in e.Added)
            {
                if (Object.Value.name is "Tapper")
                {
                    ChangeTapperMinutes(mod, Object.Value);
                }
            }
        }

        public static void ChangeTapperMinutes(ModEntry mod, Object tapper)
        {
            if (tapper.MinutesUntilReady > 1600)
            {
                tapper.MinutesUntilReady = 1600;
            }
        }
    }
}