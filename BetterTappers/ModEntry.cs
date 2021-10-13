using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace BetterTappers
{
    public class ModEntry : Mod
    {
        public ModConfig Config;

        /*  Public methods  */
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            Helper.Events.World.ObjectListChanged += this.ObjectListChanged;
        }

        /*  Private methods */
        private void ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            foreach (var Object in e.Added)
            {
                BetterTappersLogic.CheckTappers(this, Object.Value);
            }
        }
    }
}