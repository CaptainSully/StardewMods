using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using SullySDVcore.Utilities;

namespace HelpfulSpousesAndRoomates
{
    internal class Chores
    {
        /*********
        * Handy Reference List:
        * 
        *   Vanilla marriage dialogues giving items:
        *       "Indoor_Day_1": [194 195 210 211 216]   - If not raining, 1 chance in 5
        *       "Rainy_Day_2": [194 195 210 211 216]    - If raining, 1 chance in 5
        *           Fried Egg       - "(O)194"
        *           Omlete          - "(O)195"
        *           Hashbrowns      - "(O)210"
        *           Pancakes        - "(O)211"
        *           Bread           - "(O)216"
        *       
        *       "Rainy_Night_4": [199 218 219 727 730]
        *           Salad               - "(O)199"
        *           Fruit Salad         - "(O)218"
        *           Vegetable Medley    - "(O)219"
        *           Stir Fry            - "(O)727"
        *           Roasted Vegetables  - "(O)730"
        *           
        *           
        *   Vanilla chores and flags:
        *       Feed pet                - NPC.hasSomeoneFedThePet
        *       Water crops             - NPC.hasSomeoneWateredCrops
        *       Feed animals            - NPC.hasSomeoneFedTheAnimals
        *       Repair fences           - NPC.hasSomeoneRepairedTheFences
        *       Make breakfast          - none
        *       
        *   Custom chores:
        *       Pet the pet(s)
        *       Pet the animals
        *       
        *********/

        /*********
        ** Fields
        *********/
        private static readonly Log log = ModEntry.Instance.log;
        private static readonly ModConfig Config = ModEntry.Config;

        public enum ChoreType
        {
            // Vanilla
            FeedPet,
            WaterCrops,
            FeedAnimals,
            RepairFences,
            MakeBreakfast,

            // Custom
            PetPet,
            PetAnimals
        }

        // Flags
        public static bool choresHaveBeenDone = false;
        public static int choresDoneToday = 0;

        // Vanilla
        public static bool hasSomeoneFedThePet = false;
        public static bool hasSomeoneWateredCrops = false;
        public static bool hasSomeoneFedTheAnimals = false;
        public static bool hasSomeoneRepairedTheFences = false;
        public static bool hasSomeoneMadeBreakfast = false;

        // Custom
        public static bool hasSomeonePetThePet = false;
        public static bool hasSomeonePetTheAnimals = false;
        

        /*********
        ** Public methods
        *********/
        public static void ResetChores()
        {
            log.T($"Reset chore flags.");

            // Flags
            choresHaveBeenDone = false;
            choresDoneToday = 0;

            // Vanilla
            hasSomeoneFedThePet = false;
            hasSomeoneWateredCrops = false;
            hasSomeoneFedTheAnimals = false;
            hasSomeoneRepairedTheFences = false;
            hasSomeoneMadeBreakfast = false;

            // Custom
            hasSomeonePetTheAnimals = false;
            hasSomeonePetThePet = false;
        }

        public static void MakeChoresList(NPC npc)
        {
            log.T($"Make chores list for {npc.Name}.");

            if (!Config.RoommatesDoChores && npc.getSpouse().friendshipData[npc.Name].RoommateMarriage)
            {
                log.T($"Roommate chores disabled.");
                return;
            }
            if (choresHaveBeenDone && Config.OnlyOneDoesChores)
            {
                log.T($"Chores have already been done by someone today.");
                return;
            }

            Random r = new Random();
            List<int> chorelist = new List<int>();
            bool none = (Game1.isGreenRain && Config.GreenRainBehaviour == "None");
            bool insideOnly = (Game1.isGreenRain && Config.GreenRainBehaviour == "Only Inside");

            // Vanilla chores
            if (!hasSomeoneFedThePet && (Config.AlwaysFeedPet || r.NextDouble() < Config.ChanceFeedPet))
            {
                if (!Game1.isRaining) chorelist.Add((int)ChoreType.FeedPet);
            }
            if (!Game1.isRaining && !hasSomeoneWateredCrops && r.NextDouble() < Config.ChanceWaterCrops)
            {
                chorelist.Add((int)ChoreType.WaterCrops);
            }
            if (!none && !insideOnly && !hasSomeoneFedTheAnimals && r.NextDouble() < Config.ChanceFeedAnimals)
            {
                chorelist.Add((int)ChoreType.FeedAnimals);
            }
            if (!none && !insideOnly && !hasSomeoneRepairedTheFences && r.NextDouble() < Config.ChanceRepairFences)
            {
                chorelist.Add((int)ChoreType.RepairFences);
            }
            if (!none && !hasSomeoneMadeBreakfast && r.NextDouble() < Config.ChanceMakeBreakfast)
            {
                //chorelist.Add((int)ChoreType.MakeBreakfast);
            }

            // Custom chores
            if (Config.EnableCustomChores)
            {
                if (!hasSomeonePetThePet && !Config.CombinePetChores && (Config.AlwaysPetPets || (!none && r.NextDouble() < Config.ChancePetPet)))
                {
                    chorelist.Add((int)ChoreType.PetPet);
                }
                if (!none && !insideOnly && !hasSomeonePetTheAnimals && !Config.CombineAnimalChores && r.NextDouble() < Config.ChancePetAnimals)
                {
                    chorelist.Add((int)ChoreType.PetAnimals);
                }
            }
            DoChores(npc, chorelist);
        }

        public static void DoChores(NPC npc, List<int> chorelist)
        {
            if (chorelist.Count == 0)
            {
                log.T($"{npc.Name} is feeling lazy today.");
                return;
            }
            log.T($"Resolving {npc.Name}'s chores list: {chorelist.Count} listed, their max: {Config.MaxChoresPerSpouse}; max total: {Config.MaxChoresPerDay}");

            chorelist.Shuffle();

            for (int i = 0; i < chorelist.Count; i++)
            {
                if (choresDoneToday >= Config.MaxChoresPerDay)
                {
                    log.T($"Total max chores for today, skipping remaining.");
                    break;
                }
                else if (choresDoneToday >= Config.MaxChoresPerSpouse)
                {
                    log.T($"{npc.Name}'s max chores for today, skipping remaining.");
                    break;
                }
                DoChore(npc, (ChoreType)chorelist.ElementAt(i));
                log.D($"  chore num {i}", Config.Debug);

                if (!(Config.AlwaysFeedPet && chorelist.ElementAt(i) == (int)ChoreType.FeedPet) && !(Config.AlwaysPetPets && chorelist.ElementAt(i) == (int)ChoreType.PetPet))
                {
                    choresDoneToday++;
                }
                else log.D($"    didn't count toward max", Config.Debug);
            }
            choresHaveBeenDone = true;
        }

        public static void DoChore(NPC npc, ChoreType chore)
        {
            switch (chore)
            {
                case ChoreType.FeedPet:
                    FeedPet(npc);
                    if (Config.CombinePetChores) PetThePets(npc);
                    break;
                case ChoreType.WaterCrops:
                    WaterCrops(npc);
                    break;
                case ChoreType.FeedAnimals:
                    FeedAnimals(npc);
                    if (Config.CombineAnimalChores) PetTheAnimals(npc);
                    break;
                case ChoreType.RepairFences:
                    RepairFences(npc);
                    break;
                case ChoreType.MakeBreakfast:
                    MakeBreakfast(npc);
                    break;
                case ChoreType.PetPet:
                    PetThePets(npc);
                    break;
                case ChoreType.PetAnimals:
                    PetTheAnimals(npc);
                    break;
            }
        }


        /*********
        ** Vanilla chores
        *********/
        public static void FeedPet(NPC npc)
        {
            log.T($"Feeding pet");
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building is PetBowl bowl && !bowl.watered.Value)
                {
                    bowl.watered.Value = true;
                    hasSomeoneFedThePet = true;
                }
            }
            if (Utility.getAllPets().Count > 1 && Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en)
            {
                npc.addMarriageDialogue("Strings\\StringsFromCSFiles", "MultiplePetBowls_watered", false, Game1.player.getPetDisplayName());
            }
            else
            {
                npc.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4463", false, Game1.player.getPetDisplayName());
            }
        }

        public static void WaterCrops(NPC npc)
        {
            log.T($"Watering crops");

            Farm farm = Game1.getFarm();
            bool watered = false;

            foreach (var dirtTile in farm.terrainFeatures.Values.OfType<HoeDirt>())
            {
                if (dirtTile.needsWatering())
                {
                    dirtTile.state.Value = 1;
                    watered = true;
                }
            }
            if (watered)
            {
                npc.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4462", true);
            }
            else
            {
                npc.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4470", true);
            }

            hasSomeoneWateredCrops = true;
        }

        public static void FeedAnimals(NPC npc)
        {
            log.T($"Feeding animals");

            bool fedAny = false;
            foreach (Building b in Game1.getFarm().buildings)
            {
                if (b.GetIndoors() is AnimalHouse animalHouse && b.daysOfConstructionLeft.Value <= 0 && Game1.IsMasterGame)
                {
                    animalHouse.feedAllAnimals();
                    fedAny = true;
                }
            }
            if (fedAny)
            {
                npc.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4474", true);
            }

            hasSomeoneFedTheAnimals = true;
            
        }

        public static void RepairFences(NPC npc)
        {
            log.T($"Repairing fences");
            
            bool repaired = false;
            foreach (var fence in Game1.getFarm().objects.Values.OfType<Fence>())
            {
                if (fence.health.Value < fence.maxHealth.Value)
                {
                    fence.repair();
                    repaired = true;
                }
            }
            if (repaired)
            {
                npc.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4481", true);
            }

            hasSomeoneRepairedTheFences = true;
        }

        public static void MakeBreakfast(NPC npc)
        {
            log.T($"Making breakfast");
            
            if (Game1.isRaining)
            {
                npc.addMarriageDialogue("MarriageDialogue", "Rainy_Day_2", false);
                log.D($"  rainy breakfast?", Config.Debug);
            }
            else
            {
                npc.addMarriageDialogue("MarriageDialogue", "Indoor_Day_1", false);
                log.D($"  sunny breakfast?", Config.Debug);
            }

            hasSomeoneMadeBreakfast = true;
        }

        /*********
        ** Custom chores
        *********/
        public static void PetThePets(NPC npc)
        {
            log.T($"Petting pet");

            Farmer player = npc.getSpouse();
            if (player is null)
            {
                log.T($"Player is null, no petting occured");
                return;
            }

            foreach (Pet pet in Utility.getAllPets())
            {
                if (!pet.lastPetDay.TryGetValue(player.UniqueMultiplayerID, out var curLastPetDay) || curLastPetDay != Game1.Date.TotalDays)
                {
                    pet.lastPetDay[player.UniqueMultiplayerID] = Game1.Date.TotalDays;
                    if (!pet.grantedFriendshipForPet.Value)
                    {
                        pet.grantedFriendshipForPet.Set(true);
                        pet.friendshipTowardFarmer.Set(Math.Min(1000, pet.friendshipTowardFarmer.Value + 12));
                        pet.timesPet.Value++;
                    }
                }
            }

            hasSomeonePetThePet = true;
        }

        public static void PetTheAnimals(NPC npc)
        {
            log.T($"Petting animals");

            Farmer player = npc.getSpouse();
            if (player is null)
            {
                log.T($"Player is null, no petting occured");
                return;
            }
            foreach (var animal in Game1.getFarm().getAllFarmAnimals())
            {
                if (!animal.wasPet.Value)
                {
                    animal.pet(player);
                }
            }

            hasSomeonePetTheAnimals = true;
        }
    }
}
