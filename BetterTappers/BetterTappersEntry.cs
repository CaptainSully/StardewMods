namespace BetterTappers
{
    using System;
    using StardewModdingAPI;

    public class BetterTappersEntry : Mod
    {
        public BetterTappersConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<BetterTappersConfig>();
            BetterTappersConfig.VerifyConfigValues(Config, this);
        
            Helper.Events.GameLoop.GameLaunched += delegate { BetterTappersConfig.SetUpModConfigMenu(Config, this); };

            //Helper.Events.World.ObjectListChanged += this.ObjectListChanged;

            Patcher.PatchAll(this);
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }
        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();
            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";
            
            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        /*  Private methods 
        private void ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            foreach (var Object in e.Added)
            {
                BetterTappersLogic.CheckTappers(this, Object.Value);
            }
        }
        */
    }
}