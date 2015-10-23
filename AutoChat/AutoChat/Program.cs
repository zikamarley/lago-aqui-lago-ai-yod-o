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
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static List<string> Messages;
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

        private const string MenuName = "AutoChat";
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

            Chat.Print("[AutoChat Loaded]");

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
            Settings.Add("sayMessageInterval", new Slider("Minimum Interval between messages", 240, 1, 600));
        }

        static void setupEvents()
        {
            Events = new Dictionary<GameEventId, int>
            {
                { GameEventId.OnChampionKill, 1 },  // champion kill
                { GameEventId.OnTurretKill, 1 }, // turret kill

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
            Messages = new List<string>
            {
                "gj", "good job", "wp", "nice"
            };

            Smileys = new List<string>
            {
                "", "", "", "", "", ":)", ":P"
            };

            Greetings = new List<string>
            {
                "gl", "hf", "have fun guys", "gl hf", "glhf"
            };

            SignOff = new List<string>
            {
                "gg", "ggwp", "gg wp"
            };

            Apologize = new List<string>
            {
                "damn", "sorry guys", "woops", "soz"
            };

            Sympathy = new List<string>
            {
                "bl", "its ok", "next time"
            };

            Clutch = new List<string>
            {
                "hahaha", "NICE", "GJ", "WELL DONE", "HAHA", "epic", "REKT", "WOW"
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

        static string generateCongratulations(string champName)
        {
            string message = getRandomElement(Messages, false);
            message += " ";
            message += slangName(champName);
            message += " ";
            message += getRandomElement(Smileys);
            return message;
        }

        static string generateGreeting()
        {
            string greeting = getRandomElement(Greetings, false);
            greeting += " ";
            greeting += getRandomElement(Smileys);
            return greeting;
        }

        static string generateApology()
        {
            string apology = getRandomElement(Apologize, false);
            apology += " ";
            apology += getRandomElement(Smileys);
            return apology;
        }

        static string generateSympathy()
        {
            string sympathy = getRandomElement(Sympathy, false);
            sympathy += " ";
            sympathy += getRandomElement(Smileys);
            return sympathy;
        }

        static string generateClutch()
        {
            string clutch = getRandomElement(Clutch, false);
            clutch += " ";
            clutch += getRandomElement(Smileys);
            return clutch;
        }

        static string generateEnding()
        {
            string ending = getRandomElement(SignOff, false);
            ending += " ";
            ending += getRandomElement(Smileys);
            return ending;
        }

        static void sayGreeting()
        {

            if (Game.Time > 240) return;

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

        static void sayApology()
        {
            if (Settings["sayApology"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + Settings["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateApology()), (random.Next(minDelay, maxDelay)));
            }
        }

        static void saySympathy()
        {
            if (Settings["saySympathy"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + Settings["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateSympathy()), (random.Next(minDelay, maxDelay)));
            }
        }

        static void sayClutch()
        {
            if (Settings["sayCongratulate"].Cast<CheckBox>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateClutch()), (random.Next(minDelay, maxDelay)));
            }
        }

        static void saySignOff()
        {
            Core.DelayAction(() => Chat.Say("/all " + generateEnding()), (new Random(Environment.TickCount).Next(100, 1001)));
        }

        #region Slang Names
        static string slangName(string champName)
        {
            int chance = random.Next(0, 3);
            if (chance == 1)
            {
                switch (champName)
                {
                case "Aatrox": return "aatrox";
                case "Ahri": return "ahri";
                case "Akali": return "akali";
                case "Alistar": return "ali";
                case "Amumu": return "amumu";
                case "Anivia": return "anivia";
                case "Annie": return "annie";
                case "Ashe": return "ashe";
                case "Azir": return "azir";
                case "Bard": return "bard";
                case "Blitzcrank": return "blitz";
                case "Brand": return "brand";
                case "Braum": return "braum";
                case "Caitlyn": return "cait";
                case "Cassiopeia": return "cass";
                case "Cho'Gath": return "cho";
                case "Corki": return "corki";
                case "Darius": return "darius";
                case "Diana": return "diana";
                case "Dr. Mundo": return "mundo";
                case "Draven": return "draven";
                case "Ekko": return "ekko";
                case "Elise": return "elise";
                case "Evelynn": return "eve";
                case "Ezreal": return "ez";
                case "Fiddlesticks": return "fiddles";
                case "Fiora": return "fiora";
                case "Fizz": return "fizz";
                case "Galio": return "galio";
                case "Gangplank": return "gangplank";
                case "Garen": return "garen";
                case "Gnar": return "gnar";
                case "Gragas": return "gragas";
                case "Graves": return "graves";
                case "Hecarim": return "hec";
                case "Heimerdinger": return "heimer";
                case "Irelia": return "irelia";
                case "Janna": return "janna";
                case "Jarvan IV": return "j4";
                case "Jax": return "jax";
                case "Jayce": return "jayce";
                case "Jinx": return "jinx";
                case "Kalista": return "kalista";
                case "Karma": return "karma";
                case "Karthus": return "karthus";
                case "Kassadin": return "kassadin";
                case "Katarina": return "kat";
                case "Kayle": return "kayle";
                case "Kennen": return "kennen";
                case "Kha'Zix": return "khazix";
                case "Kindred": return "kindred";
                case "Kog'Maw": return "kog";
                case "LeBlanc": return "lb";
                case "Lee Sin": return "lee";
                case "Leona": return "leona";
                case "Lissandra": return "liss";
                case "Lucian": return "lucian";
                case "Lulu": return "lulu";
                case "Lux": return "lux";
                case "Malphite": return "malph";
                case "Malzahar": return "malz";
                case "Maokai": return "maokai";
                case "Master Yi": return "yi";
                case "Miss Fortune": return "mf";
                case "Mordekaiser": return "mord";
                case "Morgana": return "morg";
                case "Nami": return "nami";
                case "Nasus": return "nasus";
                case "Nautilus": return "naut";
                case "Nidalee": return "nid";
                case "Nocturne": return "noct";
                case "Nunu": return "nunu";
                case "Olaf": return "olaf";
                case "Orianna": return "orianna";
                case "Pantheon": return "pantheon";
                case "Poppy": return "poppy";
                case "Quinn": return "quinn";
                case "Rammus": return "rammus";
                case "Rek'Sai": return "reksai";
                case "Renekton": return "renekton";
                case "Rengar": return "rengar";
                case "Riven": return "riven";
                case "Rumble": return "rumble";
                case "Ryze": return "ryze";
                case "Sejuani": return "sej";
                case "Shaco": return "shaco";
                case "Shen": return "shen";
                case "Shyvana": return "shyvana";
                case "Singed": return "singed";
                case "Sion": return "sion";
                case "Sivir": return "sivir";
                case "Skarner": return "skarner";
                case "Sona": return "sona";
                case "Soraka": return "soraka";
                case "Swain": return "swain";
                case "Syndra": return "syndra";
                case "Tahm Kench": return "tahm";
                case "Talon": return "talon";
                case "Taric": return "taric";
                case "Teemo": return "teemo";
                case "Thresh": return "thresh";
                case "Tristana": return "trist";
                case "Trundle": return "trundle";
                case "Tryndamere": return "trynd";
                case "Twisted Fate": return "tf";
                case "Twitch": return "twitch";
                case "Udyr": return "udyr";
                case "Urgot": return "urgot";
                case "Varus": return "varus";
                case "Vayne": return "vayne";
                case "Veigar": return "veigar";
                case "Vel'Koz": return "velkoz";
                case "Vi": return "vi";
                case "Viktor": return "viktor";
                case "Vladimir": return "vlad";
                case "Volibear": return "voli";
                case "Warwick": return "ww";
                case "Wukong": return "wukong";
                case "Xerath": return "xerath";
                case "Xin Zhao": return "xin";
                case "Yasuo": return "yasuo";
                case "Yorick": return "yorick";
                case "Zac": return "zac";
                case "Zed": return "zed";
                case "Ziggs": return "ziggs";
                case "Zilean": return "zilean";
                case "Zyra": return "zyra";
                default: return "";
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

                // KILLS & TURRET KILLS
                if (string.Equals(args.EventId.ToString(), "OnChampionKill") || string.Equals(args.EventId.ToString(), "OnTurretKill"))
                {
                    Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(args.NetworkId);
                    AIHeroClient guy = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);
                    if (Killer.IsAlly)
                    {
                        // we will not congratulate ourselves lol :D
                        if (!Killer.IsMe)
                        {
                            sayCongratulations(guy.ChampionName);
                        }
                    }
                    else
                    {
                        saySympathy();
                    }
                }

                // DRAGON STEAL
                if (string.Equals(args.EventId.ToString(), "OnKillDragonSteal"))
                {
                    Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(args.NetworkId);

                    if (Killer.IsAlly)
                    {
                        // we will not congratulate ourselves lol :D
                        if (!Killer.IsMe)
                        {
                            sayClutch();
                        }
                    }
                }

                // QUADRAKILL
                if (string.Equals(args.EventId.ToString(), "OnChampionQuadraKill"))
                {
                    Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(args.NetworkId);

                    if (Killer.IsAlly)
                    {
                        // we will not congratulate ourselves lol :D
                        if (!Killer.IsMe)
                        {
                            sayClutch();
                        }
                    }
                }

                //PENTAKILL
                if (string.Equals(args.EventId.ToString(), "OnChampionPentaKill"))
                {
                    Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(args.NetworkId);

                    if (Killer.IsAlly)
                    {
                        // we will not congratulate ourselves lol :D
                        if (!Killer.IsMe)
                        {
                            sayClutch();
                        }
                    }
                }

                // END GAME
                if (string.Equals(args.EventId.ToString(), "OnHQDie")
                    || string.Equals(args.EventId.ToString(), "OnHQKill"))
                {
                    saySignOff();
                }
            }
        }
    }
}