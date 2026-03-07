using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using SullySDVcore.Utilities;

namespace HelpfulSpousesAndRoommates
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
        private static readonly string path = "Characters\\Dialogue\\ChoreDialogue";
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
        public static bool currentNpcDoneAny = false;
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
            currentNpcDoneAny = false;
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

            currentNpcDoneAny = false;
            Random r = new Random();
            List<int> chorelist = new List<int>();
            bool none = (Game1.isGreenRain && Config.GreenRainBehaviour == "None");
            bool insideOnly = (Game1.isGreenRain && Config.GreenRainBehaviour == "Only Inside");

            // Vanilla chores
            if (!hasSomeoneFedThePet && (Config.AlwaysFeedPet || r.NextDouble() < Config.ChanceFeedPet))
            {
                chorelist.Add((int)ChoreType.FeedPet);
                log.D($"Added {ChoreType.FeedPet} to chores list.", Config.Debug);
            }
            if (!Game1.isRaining && !hasSomeoneWateredCrops && r.NextDouble() < Config.ChanceWaterCrops)
            {
                chorelist.Add((int)ChoreType.WaterCrops);
                log.D($"Added {ChoreType.WaterCrops} to chores list.", Config.Debug);
            }
            if (!none && !insideOnly && !hasSomeoneFedTheAnimals && r.NextDouble() < Config.ChanceFeedAnimals)
            {
                chorelist.Add((int)ChoreType.FeedAnimals);
                log.D($"Added {ChoreType.FeedAnimals} to chores list.", Config.Debug);
            }
            if (!none && !insideOnly && !hasSomeoneRepairedTheFences && r.NextDouble() < Config.ChanceRepairFences)
            {
                chorelist.Add((int)ChoreType.RepairFences);
                log.D($"Added {ChoreType.RepairFences} to chores list.", Config.Debug);
            }
            if (!none && !hasSomeoneMadeBreakfast && r.NextDouble() < Config.ChanceMakeBreakfast)
            {
                chorelist.Add((int)ChoreType.MakeBreakfast);
                log.D($"Added {ChoreType.MakeBreakfast} to chores list.", Config.Debug);
            }

            // Custom chores
            if (Config.EnableCustomChores)
            {
                if (!hasSomeonePetThePet && !Config.CombinePetChores && (Config.AlwaysPetPets || (!none && r.NextDouble() < Config.ChancePetPet)))
                {
                    chorelist.Add((int)ChoreType.PetPet);
                    log.D($"Added {ChoreType.PetPet} to chores list.", Config.Debug);
                }
                if (!none && !insideOnly && !hasSomeonePetTheAnimals && !Config.CombineAnimalChores && r.NextDouble() < Config.ChancePetAnimals)
                {
                    chorelist.Add((int)ChoreType.PetAnimals);
                    log.D($"Added {ChoreType.PetAnimals} to chores list.", Config.Debug);
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
            bool didChore = false;
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
                didChore = DoChore(npc, (ChoreType)chorelist.ElementAt(i));
                if (didChore) currentNpcDoneAny = true;
                log.D($"  chore num {i}", Config.Debug);

                if (!(Config.AlwaysFeedPet && chorelist.ElementAt(i) == (int)ChoreType.FeedPet) && !(Config.AlwaysPetPets && chorelist.ElementAt(i) == (int)ChoreType.PetPet))
                {
                    if (didChore) choresDoneToday++;
                }
                else log.D($"    didn't count toward max", Config.Debug);
            }
            choresHaveBeenDone = true;
        }

        public static bool DoChore(NPC npc, ChoreType chore)
        {
            log.T($"{npc.Name} doing chore: {chore}");
            switch (chore)
            {
                case ChoreType.FeedPet:
                    if (!Config.CombinePetChores) return FeedPet(npc);
                    else return PetThePets(npc, FeedPet(npc));
                case ChoreType.WaterCrops:
                    return WaterCrops(npc);
                case ChoreType.FeedAnimals:
                    if (!Config.CombineAnimalChores) return FeedAnimals(npc);
                    else return PetTheAnimals(npc, FeedAnimals(npc));
                case ChoreType.RepairFences:
                    return RepairFences(npc);
                case ChoreType.MakeBreakfast:
                    return MakeBreakfast(npc);
                case ChoreType.PetPet:
                    return PetThePets(npc);
                case ChoreType.PetAnimals:
                    return PetTheAnimals(npc);
            }
            return false;
        }


        /*********
        ** Vanilla chores
        *********/
        public static bool FeedPet(NPC npc)
        {
            log.T($"Feeding pet");

            bool fedAny = false;
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building is PetBowl bowl && !bowl.watered.Value)
                {
                    bowl.watered.Value = true;
                    fedAny = true;
                }
            }
            if (!Config.CombinePetChores && Config.ChoreDialogue != "None" && (!currentNpcDoneAny && Config.ChoreDialogue != "First"))
            {
                if (Utility.getAllPets().Count > 1 && Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en)
                {
                    npc.addMarriageDialogue(path, "Multi_Feed_Pet");
                    log.D($"Added Multi_Feed_Pet to dialogue.", Config.Debug);
                }
                else
                {
                    npc.addMarriageDialogue(path, "Feed_Pet", false, Game1.player.getPetDisplayName());
                    log.D($"Added Feed_Pet to dialogue.", Config.Debug);
                }
            }

            hasSomeoneFedThePet = true;
            return fedAny;
        }

        public static bool WaterCrops(NPC npc)
        {
            log.T($"Watering crops");

            Farm farm = Game1.getFarm();
            bool wateredAny = false;

            foreach (var dirtTile in farm.terrainFeatures.Values.OfType<HoeDirt>())
            {
                if (dirtTile.needsWatering())
                {
                    dirtTile.state.Value = 1;
                    wateredAny = true;
                }
            }
            if (wateredAny && Config.ChoreDialogue != "None" && (!currentNpcDoneAny && Config.ChoreDialogue != "First"))
            {
                npc.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4462", true);
                log.D($"Added {ChoreType.WaterCrops} to dialogue.", Config.Debug);
            }

            hasSomeoneWateredCrops = true;
            return wateredAny;
        }

        public static bool FeedAnimals(NPC npc)
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
            if (fedAny && !Config.CombineAnimalChores && Config.ChoreDialogue != "None" && (!currentNpcDoneAny && Config.ChoreDialogue != "First"))
            {
                npc.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4474", true);
                log.D($"Added {ChoreType.FeedAnimals} to dialogue.", Config.Debug);
            }
            
            hasSomeoneFedTheAnimals = true;
            return fedAny;
        }

        public static bool RepairFences(NPC npc)
        {
            log.T($"Repairing fences");

            bool repairedAny = false;
            foreach (var fence in Game1.getFarm().objects.Values.OfType<Fence>())
            {
                if (fence.health.Value < fence.maxHealth.Value)
                {
                    fence.repair();
                    repairedAny = true;
                }
            }
            if (repairedAny && Config.ChoreDialogue != "None" && (!currentNpcDoneAny && Config.ChoreDialogue != "First"))
            {
                npc.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4481", true);
                log.D($"Added {ChoreType.RepairFences} to dialogue.", Config.Debug);
            }

            hasSomeoneRepairedTheFences = true;
            return repairedAny;
        }

        public static bool MakeBreakfast(NPC npc)
        {
            log.T($"Making breakfast");

            if (Game1.isRaining)
            {
                npc.addMarriageDialogue(path, "Rainy_Breakfast");
                log.D($"Added Rainy_Breakfast to dialogue.", Config.Debug);
            }
            else
            {
                npc.addMarriageDialogue(path, "Sunny_Breakfast");
                log.D($"Added Sunny_Breakfast to dialogue.", Config.Debug);
            }

            hasSomeoneMadeBreakfast = true;
            return true;
        }

        /*********
        ** Custom chores
        *********/
        public static bool PetThePets(NPC npc, bool fedAny = false)
        {
            log.T($"Petting pet");

            Farmer player = npc.getSpouse();
            bool petAny = false;
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
                        petAny = true;
                    }
                }
            }
            if ((petAny || fedAny) && Config.ChoreDialogue != "None" && (!currentNpcDoneAny && Config.ChoreDialogue != "First"))
            {
                if (Utility.getAllPets().Count > 1 && Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en)
                {
                    npc.addMarriageDialogue(path, !Config.CombinePetChores ? "Multi_Pet_Pet" : "Multi_Combined_Pet");
                    log.D($"Added {(!Config.CombinePetChores ? "Multi_Pet_Pet" : "Multi_Combined_Pet")} to dialogue.", Config.Debug);
                }
                else
                {
                    npc.addMarriageDialogue(path, !Config.CombinePetChores ? "Pet_Pet" : "Combined_Pet", false, Game1.player.getPetDisplayName());
                    log.D($"Added {(!Config.CombinePetChores ? "Pet_Pet" : "Combined_Pet")} to dialogue.", Config.Debug);
                }
            }

            hasSomeonePetThePet = true;
            return !Config.CombinePetChores ? petAny : (petAny || fedAny);
        }

        public static bool PetTheAnimals(NPC npc, bool fedAny = false)
        {
            log.T($"Petting animals");

            Farmer player = npc.getSpouse();
            bool petAny = false;
            foreach (var animal in Game1.getFarm().getAllFarmAnimals())
            {
                if (!animal.wasPet.Value)
                {
                    animal.pet(player);
                    petAny = true;
                }
            }
            if ((petAny || fedAny) && Config.ChoreDialogue != "None" && (!currentNpcDoneAny && Config.ChoreDialogue != "First") && !Config.CombineAnimalChores)
            {
                npc.addMarriageDialogue(path, !Config.CombinePetChores ? "Pet_Animals" : "Combined_Animals");
                log.D($"Added {(!Config.CombinePetChores ? "Pet_Animals" : "Combined_Animals")} to dialogue.", Config.Debug);
            }

            hasSomeonePetTheAnimals = true;
            return !Config.CombinePetChores ? petAny : (petAny || fedAny);
        }
    }
}
