using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTappers
{
    public class ModConfig
    {
        //options for regular tappers
        public float DaysForSyrups { get; set; } = 7;
        public float DaysForSap { get; set; } = 1;
        public float DaysForMushroom { get; set; } = 7;

        //options for hardwood tappers
        public Boolean OverrideHeavyTapperDefaults = false;
        public float DaysForSyrupsHeavy { get; set; } = (float)3.5;
        public float DaysForSapHeavy { get; set; } = (float)0.5;
        public float DaysForMushroomHeavy { get; set; } = (float)3.5;
    }
}
