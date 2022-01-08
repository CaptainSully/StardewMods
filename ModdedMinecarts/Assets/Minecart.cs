﻿
namespace ModdedMinecarts.Assets
{
    internal class Minecart
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Direction { get; set; }
        public string[] Flags { get; set; }


        public Minecart(string name, string location, int x, int y, int dir)
        {
            Name = name;
            Location = location;
            PosX = x;
            PosY = y;
            Direction = dir;
        }
    }
}