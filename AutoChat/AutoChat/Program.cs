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
        public static Dictionary<GameEventId, int> Events;
        public static Random rand = new Random();

        private const string MenuName = "AutoChat";

        public static Menu Settings;

        public static int kills = 0;
        public static int deaths = 0;
        public static float congratzTime = 0;
        public static float lastMessage = 0;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
            Loading.OnLoadingComplete += Game_OnGameStart;
            Game.OnNotify += Game_OnGameNotifyEvent;
            Game.OnUpdate += Game_OnGameUpdate;
        }

        static void setupMenu()
        {
            Settings = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Settings.AddGroupLabel("AutoChat Settings");
            Settings.Add("sayGreeting", new CheckBox("Say Greeting"));
            Settings.Add("sayGreetingAllChat", new CheckBox("Say Greeting In All Chat"));
            Settings.Add("sayGreetingDelayMin", new Slider("Min Greeting Delay", 3, 1, 5));
            Settings.Add("sayGreetingDelayMax", new Slider("Max Greeting Delay", 6, 5, 10));
            Settings.Add("sayCongratulate", new CheckBox("Congratulate players"));
            Settings.Add("sayCongratulateDelayMin", new Slider("Min Congratulate Delay", 2, 1, 3));
            Settings.Add("sayCongratulateDelayMax", new Slider("Max Congratulate Delay", 4, 3, 5));
            Settings.Add("sayCongratulateInterval", new Slider("Minimum Interval between messages", 240, 1, 600));
        }

        static void setupEvents()
        {
            Events = new Dictionary<GameEventId, int>
            {
                { GameEventId.OnChampionKill, 1 },  // champion kill
                { GameEventId.OnTurretKill, 1 }, // turret kill
                { GameEventId.OnHQDie, 1 },  // nexus die
                { GameEventId.OnHQKill, 1 },  // nexus kill
                { GameEventId.OnSurrenderAgreed, 1 },  // agree surrender
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

        static string generateSignOff()
        {
            string signoff = getRandomElement(SignOff, false);
            signoff += getRandomElement(Smileys);
            return signoff;
        }

        static void sayCongratulations()
        {
            if (Settings["sayCongratulate"].Cast<CheckBox>().CurrentValue && Game.Time > lastMessage + Settings["sayCongratulateInterval"].Cast<Slider>().CurrentValue)
            {
                lastMessage = Game.Time;
                Chat.Say(generateMessage());
            }
        }

        static void sayGreeting()
        {
            if (Settings["sayGreetingAllChat"].Cast<CheckBox>().CurrentValue)
            {
                Chat.Say("/all " + generateGreeting());
            }
            else
            {
                Chat.Say(generateGreeting());
            }
        }

        static void saySignOff()
        {
            Core.DelayAction(() => Chat.Say(generateSignOff()), (new Random(Environment.TickCount).Next(100, 1001)));
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            setupMenu();
            setupMessages();
            setupEvents();
            Chat.Print("[AutoChat Loaded]");
        }


        static void Game_OnGameStart(EventArgs args)
        {
            if (!Settings["sayGreeting"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            int minDelay = Settings["sayGreetingDelayMin"].Cast<Slider>().CurrentValue;
            int maxDelay = Settings["sayGreetingDelayMax"].Cast<Slider>().CurrentValue;

            // greeting message
            Core.DelayAction(sayGreeting, rand.Next(Math.Min(minDelay, maxDelay), Math.Max(minDelay, maxDelay)) * 1000);
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            // champ kill message
            if (kills > deaths && congratzTime < Game.Time && congratzTime != 0)
            {
                sayCongratulations();

                kills = 0;
                deaths = 0;
                congratzTime = 0;
            }
            else if (kills != deaths && congratzTime < Game.Time)
            {
                kills = 0;
                deaths = 0;
                congratzTime = 0;
            }
        }

        static void Game_OnGameNotifyEvent(GameNotifyEventArgs args)
        {
            if (Events.ContainsKey(args.EventId))
            {
                // END GAME
                if (string.Equals(args.EventId.ToString(), "OnHQDie") 
                    || string.Equals(args.EventId.ToString(), "OnHQKill")
                    || string.Equals(args.EventId.ToString(), "OnSurrenderAgreed"))
                {
                    saySignOff();
                    return;
                }

                // KILLS & TURRET KILLS
                if (string.Equals(args.EventId.ToString(), "OnChampionKill") || string.Equals(args.EventId.ToString(), "OnTurretKill"))
                {
                    Obj_AI_Base Killer = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)args.NetworkId);

                    if (Killer.IsAlly)
                    {
                        // we will not congratulate ourselves lol :D
                        if ((Killer.IsMe) || kills > 0)
                        {
                            return;
                        }
                        kills += Events[args.EventId];
                    }
                    else
                    {
                        deaths += Events[args.EventId];
                    }
                }
                else
                {
                    return;
                }

                int minDelay = Settings["sayCongratulateDelayMin"].Cast<Slider>().CurrentValue;
                int maxDelay = Settings["sayCongratulateDelayMax"].Cast<Slider>().CurrentValue;

                congratzTime = Game.Time + rand.Next(Math.Min(minDelay, maxDelay), Math.Max(minDelay, maxDelay));
            }
        }
    }
}