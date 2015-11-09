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
        public static List<string> Honor;

        public static Dictionary<GameEventId, int> Events;
        public static Random random = new Random();

        public static float lastMessage = 0;
        public static float motivatemessage = 0;
        public static int minDelay = 0;
        public static int maxDelay = 0;
        
        public const string MenuName = "AutoChat";
        public static Menu BaseMenu, GreetingMenu, OptionsMenu, EndGameMenu;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            setupMenu();
            setupEvents();
            setupMessages();

            Console.WriteLine("AutoChat - Loaded");
            sayGreeting();
            Game.OnNotify += Game_OnGameNotifyEvent;
        }

        static void setupMenu()
        {
            BaseMenu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            BaseMenu.Add("enable", new CheckBox("Enable Autochat"));

            GreetingMenu = BaseMenu.AddSubMenu("Greeting Message", "greetingmessage");
            GreetingMenu.Add("sayGreeting", new CheckBox("Say Greeting"));
            GreetingMenu.Add("sayGreetingAllChat", new CheckBox("Say Greeting In All Chat"));
            GreetingMenu.AddSeparator();
            GreetingMenu.Add("sayGreetingDelayMin", new Slider("Min Greeting Delay", 3, 1, 5));
            GreetingMenu.Add("sayGreetingDelayMax", new Slider("Max Greeting Delay", 6, 5, 10));

            OptionsMenu = BaseMenu.AddSubMenu("Message Options", "messageoptions");
            OptionsMenu.Add("sayCongratulate", new CheckBox("Congratulate"));
            OptionsMenu.Add("sayMotivate", new CheckBox("Motivate"));
            OptionsMenu.Add("sayApology", new CheckBox("Apologize"));
            OptionsMenu.Add("saySympathy", new CheckBox("Sympathetic"));
            OptionsMenu.Add("sayHonor", new CheckBox("Honor Opponents"));
            OptionsMenu.AddSeparator();
            OptionsMenu.Add("sayMessageDelayMin", new Slider("Min Message Delay", 2, 1, 3));
            OptionsMenu.Add("sayMessageDelayMax", new Slider("Max Message Delay", 6, 3, 10));
            OptionsMenu.Add("sayMessageInterval", new Slider("Minimum Interval between messages", 180, 1, 600));

            EndGameMenu = BaseMenu.AddSubMenu("EndGame Message", "endgamemessage");
            EndGameMenu.Add("sayEndGame", new CheckBox("Say EndGame Message"));
            EndGameMenu.Add("sayEndGameAllChat", new CheckBox("Say EndGame Message In All Chat"));
        }

        static void setupEvents()
        {
            Events = new Dictionary<GameEventId, int>
            {
                { GameEventId.OnChampionKill, 1 },  // champion kill
                { GameEventId.OnTurretKill, 1 }, // turret kill
                { GameEventId.OnKillDragon, 1 }, // kill dragon
                { GameEventId.OnKillWorm, 1 }, // kill baron
                { GameEventId.OnChampionDie, 1},  // you die
                { GameEventId.OnDeathAssist,1 }, // assist
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
                "bl", "bl", "tough luck", "bad luck", "bummer"
            };

            Clutch = new List<string>
            {
                "hahaha", "NICE", "GJ", "WELL DONE", "HAHA", "epic", "REKT", "WOW", "GG"
            };

            Motivate = new List<string>
            {
                "push towers", "towers" , "clear lanes", "push", "need objectives", "objectives" ,"lanes", "push", "drag?"
            };

            Honor = new List<string>
            {
                "wp" ,"wp" , "wp sir", "damn wp", "wp mate", "wp dude", "wp man"
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

        static string generateCongratulations(AIHeroClient champ)
        {
            string message = getRandomElement(Congrats, false);
            message += " ";
            message += slangName(champ.ChampionName);
            message += " ";
            message += getRandomElement(Smileys);
            return message;
        }

        static string generateCongratulations()
        {
            string message = getRandomElement(Congrats, false);
            return message;
        }

        static string generateSympathy(AIHeroClient champ)
        {
            string sympathy = getRandomElement(Sympathy, false);
            sympathy += " ";
            sympathy += slangName(champ.ChampionName);
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

        static string generateHonor()
        {
            string honor = getRandomElement(Honor, false);
            honor += " ";
            honor += getRandomElement(Smileys);
            return honor;
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

            int minGreetingDelay = GreetingMenu["sayGreetingDelayMin"].Cast<Slider>().CurrentValue * 1000;
            int maxGreetingDelay = GreetingMenu["sayGreetingDelayMax"].Cast<Slider>().CurrentValue * 1000;

            if (GreetingMenu["sayGreeting"].Cast<CheckBox>().CurrentValue)
            {
                if (GreetingMenu["sayGreetingAllChat"].Cast<CheckBox>().CurrentValue)
                {
                    Core.DelayAction(() => Chat.Say("/all " + generateGreeting()), (random.Next(minGreetingDelay, maxGreetingDelay)));
                }
                else
                {
                    Core.DelayAction(() => Chat.Say(generateGreeting()), (random.Next(minGreetingDelay, maxGreetingDelay)));
                }
                
            }
        }

        static void sayCongratulations(AIHeroClient champ)
        {
            if (OptionsMenu["sayCongratulate"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + OptionsMenu["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() =>
                {
                    if (champ.IsDead) return;
                    Chat.Say(generateCongratulations(champ));
                }, (random.Next(minDelay, maxDelay)));
            }
        }

        static void sayCongratulations()
        {
            if (OptionsMenu["sayCongratulate"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + OptionsMenu["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() =>
                {
                    Chat.Say(generateCongratulations());
                }, (random.Next(minDelay, maxDelay)));
            }
        }

        static void saySympathy(AIHeroClient champ)
        {
            if (OptionsMenu["saySympathy"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + OptionsMenu["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateSympathy(champ)), (random.Next(minDelay, maxDelay)));
            }
        }

        static void sayApology()
        {
            if (OptionsMenu["sayApology"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + OptionsMenu["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateApology()), (random.Next(minDelay, maxDelay)));
            }
        }

        static void sayClutch()
        {
            if (OptionsMenu["sayCongratulate"].Cast<CheckBox>().CurrentValue)
            {
                Core.DelayAction(() => Chat.Say(generateClutch()), (random.Next(minDelay, maxDelay)));
            }
        }

        static void sayMotivation()
        {
            if (OptionsMenu["sayMotivate"].Cast<CheckBox>().CurrentValue && Game.Time > motivatemessage)
            {
                motivatemessage = Game.Time + 60;
                Core.DelayAction(() => Chat.Say(generateCongratulations() + " " + generateMotivation()), (random.Next(minDelay, maxDelay)));
            }
        }

        static void sayHonor()
        {
            if (OptionsMenu["sayHonor"].Cast<CheckBox>().CurrentValue)
            {
                if (chance(10))
                {
                    Core.DelayAction(() => Chat.Say("/all " + generateHonor()), (random.Next(minDelay, maxDelay)));
                }
            }
        }

        static void saySignOff()
        {
            if (EndGameMenu["sayEndGame"].Cast<CheckBox>().CurrentValue)
            {
                if (EndGameMenu["sayEndGameAllChat"].Cast<CheckBox>().CurrentValue)
                {
                    Core.DelayAction(() => Chat.Say("/all " + generateEnding()), (new Random(Environment.TickCount).Next(100, 1001)));
                    return;
                }
                Core.DelayAction(() => Chat.Say(generateEnding()), (new Random(Environment.TickCount).Next(100, 1001)));
            }
        }
        #endregion

        static public bool chance(int percentchance)
        {
            int p = (100 / percentchance);
            int c = random.Next(0, p);
            if (c == 1) return true;
            return false;
        }

        #region Slang Names
        static string slangName(string champName)
        {
            if (chance(20))
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
                    case "MasterYi": return "yi";
                    case "Miss Fortune": return "mf";
                    case "MonkeyKing": return "wk";
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
            if (!BaseMenu["enable"].Cast<CheckBox>().CurrentValue) return;

            if (Events.ContainsKey(args.EventId))
            {
                minDelay = OptionsMenu["sayMessageDelayMin"].Cast<Slider>().CurrentValue * 1000;
                maxDelay = OptionsMenu["sayMessageDelayMax"].Cast<Slider>().CurrentValue * 1000;

                // CHAMPION KILLS
                if (args.EventId.ToString() == "OnChampionKill" || args.EventId.ToString() == "OnTurretKill")
                {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);
                    if (_killer.IsAlly)
                    {
                        if (!_killer.IsMe)
                        {
                            sayCongratulations(_killer);
                        }
                    }
                }

                //TURRET KILLS
                if (args.EventId.ToString() == "OnTurretKill")
                {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);
                    if (_killer.IsAlly)
                    {
                        if (!_killer.IsMe)
                        {
                            sayCongratulations();
                        }
                    }
                }

                // OBJECTIVES
                if (args.EventId.ToString() == "OnKillDragon" || args.EventId.ToString() == "OnKillWorm")
                {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);
                    if (_killer.IsAlly)
                    {
                        if (!_killer.IsMe)
                        {
                            sayCongratulations();
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
                            sayHonor();
                        }
                        else
                        {
                            saySympathy(_dead);
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