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
        public static Random rand = new Random();
        public static float lastMessage = 0;
        public static float messageTime = 0;
        public static int minDelay = 0;
        public static int maxDelay = 0;

        private const string MenuName = "AutoChat";
        public static Menu Settings;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
            Game.OnNotify += Game_OnGameNotifyEvent;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            setupMenu();
            setupEvents();
            setupMessages();

            Chat.Print("[AutoChat Loaded]");
            sayGreeting();
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
                "", "", "", " :)", " :P"
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
            if (firstEmpty && rand.Next(3) == 0)
                return collection[0];

            return collection[rand.Next(collection.Count)];
        }

        static string generateMessage()
        {
            string message = getRandomElement(Messages, false);
            message += getRandomElement(Smileys);
            return message;
        }

        static string generateGreeting()
        {
            string greeting = getRandomElement(Greetings, false);
            greeting += getRandomElement(Smileys);
            return greeting;
        }

        static string generateApology()
        {
            string apology = getRandomElement(Apologize, false);
            apology += getRandomElement(Smileys);
            return apology;
        }

        static string generateSympathy()
        {
            string sympathy = getRandomElement(Sympathy, false);
            sympathy += getRandomElement(Smileys);
            return sympathy;
        }

        static string generateClutch()
        {
            string clutch = getRandomElement(Clutch, false);
            clutch += getRandomElement(Smileys);
            return clutch;
        }

        static string generateSignOff()
        {
            string signoff = getRandomElement(SignOff, false);
            signoff += getRandomElement(Smileys);
            return signoff;
        }

         static void sayGreeting()
        {
            int minGreetingDelay = Settings["sayGreetingDelayMin"].Cast<Slider>().CurrentValue * 1000;
            int maxGreetingDelay = Settings["sayGreetingDelayMax"].Cast<Slider>().CurrentValue * 1000;

            if (Settings["sayGreeting"].Cast<CheckBox>().CurrentValue)
            {
                if (Settings["sayGreetingAllChat"].Cast<CheckBox>().CurrentValue)
                {
                    Core.DelayAction(() => Chat.Say("/all " + generateGreeting()), (rand.Next(minGreetingDelay, maxGreetingDelay)));
                    return;
                }
                Core.DelayAction(() => Chat.Say(generateGreeting()), (rand.Next(minGreetingDelay, maxGreetingDelay)));
            }

            
        }

        static void sayCongratulations()
        {
            if (Settings["sayCongratulate"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + Settings["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                Core.DelayAction(() => Chat.Say(generateMessage()), (rand.Next(minDelay * 1000, maxDelay * 1000)));
            }
        }

        static void sayApology()
        {
            if (Settings["sayApology"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + Settings["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateApology()), (rand.Next(minDelay * 1000, maxDelay * 1000)));
            }
        }

        static void saySympathy()
        {
            if (Settings["saySympathy"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + Settings["sayMessageInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateSympathy()), (rand.Next(minDelay * 1000, maxDelay * 1000)));
            }
        }

        static void sayClutch()
        {
            if (Settings["sayCongratulate"].Cast<CheckBox>().CurrentValue)
            {
                lastMessage = Game.Time;
                Core.DelayAction(() => Chat.Say(generateClutch()), (rand.Next(minDelay * 1000, maxDelay * 1000)));
            }
        }

        static void saySignOff()
        {
            Core.DelayAction(() => Chat.Say("/all " + generateSignOff()), (new Random(Environment.TickCount).Next(100, 1001)));
        }

        static void Game_OnGameNotifyEvent(GameNotifyEventArgs args)
        {
            if (Events.ContainsKey(args.EventId))
            {
                minDelay = Settings["sayMessageDelayMin"].Cast<Slider>().CurrentValue;
                maxDelay = Settings["sayMessageDelayMax"].Cast<Slider>().CurrentValue;

                // END GAME
                if (string.Equals(args.EventId.ToString(), "OnHQDie") 
                    || string.Equals(args.EventId.ToString(), "OnHQKill"))
                {
                    saySignOff();
                }

                // KILLS & TURRET KILLS
                if (string.Equals(args.EventId.ToString(), "OnChampionKill") || string.Equals(args.EventId.ToString(), "OnTurretKill"))
                {
                    Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(args.NetworkId);
                    
                    if (Killer.IsAlly)
                    {
                        // we will not congratulate ourselves lol :D
                        if (!Killer.IsMe)
                        {
                            sayCongratulations();
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

                messageTime = Game.Time + rand.Next(minDelay, maxDelay);
            }
        }
    }
}