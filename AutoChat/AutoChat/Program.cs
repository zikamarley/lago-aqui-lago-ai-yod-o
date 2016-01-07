using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;

namespace AutoChat {
    class Program {
        public static AIHeroClient Player = EloBuddy.Player.Instance;

        public static List<string> Penta;
        public static List<string> Quadra;
        public static List<string> Death;
        public static List<string> MyFirstKill;
        public static Dictionary<GameEventId, int> Events;
        public static Random Rand = new Random();
        
        public static int cancerPenta = 0;
        public static int cancerQuadra = 0;
        public static int cancerKill = 0;
        public static int totalKills = 0;
        
        
        static void SetupEvents() {
            Events = new Dictionary<GameEventId, int> {
                { GameEventId.OnChampionKill, 1 },  // champion kill
                { GameEventId.OnChampionDie, 1},  // you die
                { GameEventId.OnChampionQuadraKill, 1 }, // quadra kill
                { GameEventId.OnChampionPentaKill, 1 }, // penta kill
            };
        }
        
        
        static void SetupMessages() {
            Greetings = new List<string> {
                "lago ai lago aqui?",
                "ÉÔQQQQQQQQQQQQ",
                "vem pro duelo"
            };
            
            Penta = new List<string> {
				"circo de SOLÉÉÉÉD",
				"garanta já seu ingresso para o circo de SOLÉD",
				"circo de soled kappa",
				"cos corpinho no chão da pa faze um cimitério"
            };
            
            Quadra = new List<string> {
				"vem pro pentaaaaaaa lixo",
				"trabson parsa",
				"RX RX RX RX RX RX"
            };

            MyFirstKill = new List<string> {
                "ESSE SAPATO AI É SO LADO?",
                "PLASMA?????/",
				"É DE PLASMA OU SOLED?"
            };
        }
        
        static void Game_OnGameLoad(EventArgs args) {
            SetupEvents();
            SetupMessages();
		}

        static string GetRandomElement(List<string> collection, bool firstEmpty = true) {
            if (firstEmpty && Rand.Next(3) == 0)
                return collection[0];

            return collection[Rand.Next(collection.Count)];
        }
        
        
        
        static void Game_OnGameStart(EventArgs args) {
            Core.DelayAction(cancerGreeting, 2000);
        }
		
		static void Game_OnGameNotifyEvent(GameNotifyEventArgs args) {
            if(Events.ContainsKey(args.EventId)) {
                if(args.EventId.ToString() == "OnChampionKill") {
                    totalKills++;
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);

				    if(killer.IsAlly && killer.IsMe) {
                        if(totalKills == 1) {
                            Core.DelayAction(cancerFirstBlood, 2000);
                            cancerKill = 1;
                            return;
					    }
					    if(cancerKill == 0) {
                            Core.DelayAction(cancerFirstKill, 2000);
                            cancerKill = 1;
					    }
					}
                }
                
                
                if(args.EventId.ToString() == "OnChampionPentaKill") {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);
                    
                    if(killer.IsAlly && killer.IsMe) {
						cancerPenta = 1;
                        Core.DelayAction(cancerPentaKill, 500);
					}
                }
                
                if(args.EventId.ToString() == "OnChampionQuadraKill") {
                    AIHeroClient _killer = ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId);

                    if(killer.IsAlly && killer.IsMe) {
						cancerQuadra = 1;
                        Core.DelayAction(cancerQuadraKill, 2000);
					}
                }
			}
		}
		static void cancerGreeting() {
            if(Rand.Next(3) < 1) {
				return;
			}

            Chat.Say(GetRandomElement(Greetings, false));
		}
		static void cancerFirstBlood() {
            Chat.Say("hj eu to embrasado", false);
            
            if(Rand.Next(3) == 0) {
				Chat.Say("kappa");
            }
		}
		static void cancerFirstKill() {
            if(Rand.Next(3) == 0) {
				return;
			}
			
            Chat.Say(GetRandomElement(MyFirstKill, false));
            
            if(Rand.Next(3) == 0) {
				Chat.Say("KAPPA");
            }
		}
		static void cancerQuadraKill() {
			if(cancerPenta == 1) {
                cancerQuadra = 0;
				return;
		    }

            Chat.Say(GetRandomElement(Quadra, false));
            cancerQuadra = 0;
		}
		static void cancerPentaKill() {
			if(cancerQuadra == 0) {
                cancerPenta = 0;
				return;
		    }
			
            Chat.Say(GetRandomElement(Penta, false));
            cancerPenta = 0;
		}
    }
}













