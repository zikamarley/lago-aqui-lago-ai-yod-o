using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;

namespace AutoChat
{
    class Program
    {
        public static List<string> Congrats;
        public static List<string> Smileys;
        public static List<string> Greetings;
        public static List<string> SignOff;
        public static List<string> Apologize;
        public static List<string> Sympathy;
        public static List<string> Clutch;
        public static List<string> Motivate;

        public static Dictionary<GameEventId, int> Events;
        public static Random random = new Random();

        public static float lastMessage = 0;
        public static int minDelay = 0;
        public static int maxDelay = 0;

        

        public const string MenuName = "AutoChat";
        public static Menu Settings;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            setupMenu();
            setupEvents();
            setupMessages();

            Console.WriteLine("[AutoChat Loaded]");
            sayGreeting();
            Game.OnNotify += Game_OnGameNotifyEvent;
        }

        static void setupMenu()
        {
            Settings = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Settings.AddGroupLabel("AutoChat Settings");
            Settings.Add("sayGreeting", new CheckBox("Say Greeting"));
            Settings.Add("sayGreetingAllChat", new CheckBox("Say Greeting In All Chat"));
            Settings.Add("sayGreetingDelayMin", new Slider("Min Greeting Delay", 3, 1, 5));
            Settings.Add("sayGreetingDelayMax", new Slider("Max Greeting Delay", 6, 5, 10));
            Settings.AddSeparator();
            Settings.Add("sayCongratulate", new CheckBox("Congratulate players"));
            Settings.Add("sayMotivate", new CheckBox("Motivate Players"));
            Settings.Add("sayApology", new CheckBox("Apologize to players on deaths"));
            Settings.Add("saySympathy", new CheckBox("Sympathetic on deaths"));
            Settings.AddSeparator();
            Settings.Add("sayMessageDelayMin", new Slider("Min Message Delay", 2, 1, 3));
            Settings.Add("sayMessageDelayMax", new Slider("Max Message Delay", 4, 3, 5));
            Settings.Add("sayMessageInterval", new Slider("Minimum Interval between messages", 180, 1, 600));
        }

        static void setupEvents()
        {
            Events = new Dictionary<GameEventId, int>
            {
                { GameEventId.OnChampionKill, 1 },  // champion kill
                { GameEventId.OnTurretKill, 1 }, // turret kill
                { GameEventId.OnDie, 1 },
                { GameEventId.OnKill, 1 },
                { GameEventId.OnChampionDie, 1},  // you die
                { GameEventId.OnChampionQuadraKill, 1 }, // quadra kill
                { GameEventId.OnChampionPentaKill, 1 }, // penta kill
                { GameEventId.OnAce, 1}, // ace enemy team
                { GameEventId.OnKillDragonSteal, 1 }, // dragon steal
                { GameEventId.OnReconnect, 1 }, // on reconnect
                { GameEventId.OnHQDie, 1 },  // nexus die
                { GameEventId.OnHQKill, 1 },  // nexus kill
            };
        }

        static void setupMessages()
        {
            Congrats = new List<string>
            {
                "gj", "gj", "gj", "gj", "good job", "wp", "nice", "goodjob", "gjwp", "nice one", "sweet", "awesome"
            };

            Smileys = new List<string>
            {
                "", "", "", "", "", ":)", ":P"
            };

            Greetings = new List<string>
            {
                "gl", "hf", "have fun guys", "gl hf", "glhf", "have fun all"
            };

            SignOff = new List<string>
            {
                "gg", "ggwp", "gg wp"
            };

            Apologize = new List<string>
            {
                "damn", "sorry guys", "sorry", "woops", "soz", "my bad"
            };

            Sympathy = new List<string>
            {
                "bl", "tough luck", "bad luck", "bummer"
            };

            Clutch = new List<string>
            {
                "hahaha", "NICE", "GJ", "WELL DONE", "HAHA", "epic", "REKT", "WOW", "GG"
            };

            Motivate = new List<string>
            {
                "push towers", "clear lanes", "push", "need objectives"
            };
        }

        static string getRandomElement(List<string> collection, bool firstEmpty = true)
        {
            return collection[random.Next(collection.Count)];
        }

        #region Generate Functions
        static string generateGreeting()
        {
            string greeting = getRandomElement(Greetings, false);
            greeting += " ";
            greeting += getRandomElement(Smileys);
            return greeting;
        }

        static string generateCongratulations(string champName)
        {
            string message = getRandomElement(Congrats, false);
            message += " ";
            message += slangName(champName);
            message += " ";
            message += getRandomElement(Smileys);
            return message;
        }

        static string generateSympathy(string champName)
        {
            string sympathy = getRandomElement(Sympathy, false);
            sympathy += " ";
            sympathy += slangName(champName);
            sympathy += " ";
            sympathy += getRandomElement(Smileys);
            return sympathy;
        }

        static string generateApology()
        {
            string apology = getRandomElement(Apologize, false);
            apology += " ";
            apology += getRandomElement(Smileys);
            return apology;
        }

        
        static string generateClutch()
        {
            string clutch = getRandomElement(Clutch, false);
            clutch += " ";
            clutch += getRandomElement(Smileys);
            return clutch;
        }
        
        static string generateMotivation()
        {
            string motivate = getRandomElement(Motivate, false);
            motivate += " ";
            motivate += getRandomElement(Smileys);
            return motivate;
        }

        static string generateEnding()
        {
            string ending = getRandomElement(SignOff, false);
            ending += " ";
            ending += getRandomElement(Smileys);
            return ending;
        }
        #endregion

        #region Say Functions
        static void sayGreeting()
        {

            if (Game.Time > 300) return;

            int minGreetingDelay = Settings["sayGreetingDelayMin"].Cast<Slider>().CurrentValue * 1000;
            int maxGreetingDelay = Settings["sayGreetingDelayMax"].Cast<Slider>().CurrentValue * 1000;

            if (Settings["sayGreeting"].Cast<CheckBox>().CurrentValue)
            {
                if (Settings["sayGreetingAllChat"].Cast<CheckBox>().CurrentValue)
                {
                    Core.DelayAction(() => Chat.Say("/all " + generateGreeting()), (random.Next(minGreetingDelay, maxGreetingDelay)));
                    return;
                }
                Core.DelayAction(() => Chat.Say(generateGreeting()), (random.Next(minGreetingDelay, maxGreetingDelay)));
            }
        }

        static void sayCongratulations(string champName)
        {
            if (Settings["sayCongratulate"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + Settings["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateCongratulations(champName)), (random.Next(minDelay, maxDelay)));
            }
        }

        static void saySympathy(string champName)
        {
            if (Settings["saySympathy"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + Settings["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateSympathy(champName)), (random.Next(minDelay, maxDelay)));
            }
        }

        static void sayApology()
        {
            if (Settings["sayApology"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + Settings["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateApology()), (random.Next(minDelay, maxDelay)));
            }
        }
        
        static void sayClutch()
        {
            if (Settings["sayCongratulate"].Cast<CheckBox>().CurrentValue)
            {
                Core.DelayAction(() => Chat.Say(generateClutch()), (random.Next(minDelay, maxDelay)));
            }
        }

        static void sayMotivation()
        {
            if (Settings["sayMotivate"].Cast<CheckBox>().CurrentValue)
            {
                Core.DelayAction(() => Chat.Say(generateMotivation()), (random.Next(minDelay, maxDelay)));
            }
        }

        static void saySignOff()
        {
            Core.DelayAction(() => Chat.Say("/all " + generateEnding()), (new Random(Environment.TickCount).Next(100, 1001)));
        }
        #endregion

        #region Slang Names
        static string slangName(string champName)
        {
            int chance = random.Next(0, 5);
            if (chance == 1)
            {
                switch (champName)
                {
                    case "Alistar": return "ali";
                    case "Blitzcrank": return "blitz";
                    case "Caitlyn": return "cait";
                    case "Cassiopeia": return "cass";
                    case "Cho'Gath": return "cho";
                    case "Dr. Mundo": return "mundo";
                    case "Evelynn": return "eve";
                    case "Ezreal": return "ez";
                    case "Fiddlesticks": return "fiddles";
                    case "Gangplank": return "gp";
                    case "Hecarim": return "hec";
                    case "Heimerdinger": return "heimer";
                    case "Jarvan IV": return "j4";
                    case "Katarina": return "kat";
                    case "Kha'Zix": return "khazix";
                    case "Kog'Maw": return "kog";
                    case "LeBlanc": return "lb";
                    case "Lee Sin": return "lee";
                    case "Lissandra": return "liss";
                    case "Malphite": return "malph";
                    case "Malzahar": return "malz";
                    case "Master Yi": return "yi";
                    case "Miss Fortune": return "mf";
                    case "Mordekaiser": return "mord";
                    case "Morgana": return "morg";
                    case "Nautilus": return "naut";
                    case "Nidalee": return "nid";
                    case "Nocturne": return "noct";
                    case "Orianna": return "ori";
                    case "Rek'Sai": return "reksai";
                    case "Sejuani": return "sej";
                    case "Tahm Kench": return "tahm";
                    case "Tristana": return "trist";
                    case "Tryndamere": return "trynd";
                    case "Twisted Fate": return "tf";
                    case "Vel'Koz": return "velkoz";
                    case "Vladimir": return "vlad";
                    case "Volibear": return "voli";
                    case "Warwick": return "ww";
                    case "Xin Zhao": return "xin";
                    default: return champName.ToLower();
                }
            }
            return "";
        }
        #endregion

        static void Game_OnGameNotifyEvent(GameNotifyEventArgs args)
        {
            if (Events.ContainsKey(args.EventId))
            {
                minDelay = Settings["sayMessageDelayMin"].Cast<Slider>().CurrentValue * 1000;
                maxDelay = Settings["sayMessageDelayMax"].Cast<Slider>().CurrentValue * 1000;

                // KILLS
                if (args.EventId.ToString() == "OnChampionKill")
                {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);
                    if (_killer.IsAlly)
                    {
                        if (!_killer.IsMe)
                        {
                            sayCongratulations(_killer.ChampionName);
                        }
                    }
                }

                // DEATHS
                if (args.EventId.ToString() == "OnChampionDie")
                {
                    AIHeroClient _dead = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);
                    if (_dead.IsAlly)
                    {
                        if (_dead.IsMe)
                        {
                            sayApology();
                        }
                        else
                        {
                            saySympathy(_dead.ChampionName);
                        }
                    }
                }

                // DRAGON STEAL
                if (string.Equals(args.EventId.ToString(), "OnKillDragonSteal"))
                {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);

                    if (_killer.IsAlly)
                    {
                        if (!_killer.IsMe)
                        {
                            sayClutch();
                        }
                    }
                }

                // QUADRAKILL
                if (args.EventId.ToString() == "OnChampionQuadraKill")
                {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);

                    if (_killer.IsAlly)
                    {
                        if (!_killer.IsMe)
                        {
                            sayClutch();
                        }
                    }
                }

                //PENTAKILL
                if (args.EventId.ToString() == "OnChampionPentaKill")
                {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);

                    if (_killer.IsAlly)
                    {
                        if (!_killer.IsMe)
                        {
                            sayClutch();
                        }
                    }
                }

                //ACE
                if (args.EventId.ToString() == "OnAce")
                {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);
                    
                    if (_killer.IsAlly)
                    {
                        sayMotivation();
                    }
                }

                // END GAME
                if (args.EventId.ToString() == "OnHQDie" || args.EventId.ToString() == "OnHQKill")
                {
                    saySignOff();
                }
            }
        }
    }
}