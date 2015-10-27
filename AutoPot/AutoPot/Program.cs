using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;

namespace AutoPot
{
    class Program
    {
        const int HealthPot = (int)2003;
        const int ManaPot = (int)2004;
        const int Biscuit = (int)2010;
        const int Flask = (int)2041;

        private const string MenuName = "AutoPot";
        public static Menu Settings;

        static void Main(string[] args)
        {
            try
            {
                Loading.OnLoadingComplete += OnLoad;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void OnLoad(EventArgs args)
        {
            Console.WriteLine("AutoPot - Loaded");
            CreateMenu();
            Game.OnUpdate += OnUpdate;
        }

        static Obj_AI_Base Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        static void CreateMenu()
        {
            Settings = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Settings.AddGroupLabel("AutoPot Settings");
            Settings.Add("enable", new CheckBox("Enable"));
            Settings.AddSeparator();
            Settings.Add("use.hp_potion", new CheckBox("Use Health Potions"));
            Settings.Add("use.mana_potion", new CheckBox("Use Mana Potions"));
            Settings.Add("use.biscuit", new CheckBox("Use Biscuits"));
            Settings.Add("use.flask", new CheckBox("Use Flasks"));
            Settings.Add("use.on_health_percent", new Slider("Use Health Pots if Health < x%", 50, 0, 100));
            Settings.Add("use.on_mana_percent", new Slider("Use Mana Pots if Mana < x%", 50, 0, 100));
        }

        static bool IsPotRunning()
        {
            return
            Player.HasBuff("ItemMiniRegenPotion")
            || Player.HasBuff("ItemCrystalFlask")
            || Player.HasBuff("RegenerationPotion")
            || Player.HasBuff("FlaskOfCrystalWater")
            ;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Settings["enable"].Cast<CheckBox>().CurrentValue) return;

            if (IsPotRunning()) return;

            if (InFountain(Player)) return;

            if (Player.HealthPercent <= Settings["use.on_health_percent"].Cast<Slider>().CurrentValue)
            {
                if (Settings["use.hp_potion"].Cast<CheckBox>().CurrentValue && Item.HasItem(HealthPot))
                    Item.UseItem(HealthPot);
                else
                    if (Settings["use.biscuit"].Cast<CheckBox>().CurrentValue)
                    Item.UseItem(Biscuit);
            }

            if (Settings["use.mana_potion"].Cast<CheckBox>().CurrentValue && Player.ManaPercent <= Settings["use.on_mana_percent"].Cast<Slider>().CurrentValue)
            {
                Item.UseItem(ManaPot);
            }

            if ((Settings["use.flask"].Cast<CheckBox>().CurrentValue && Player.HealthPercent <= Settings["use.on_health_percent"].Cast<Slider>().CurrentValue)
             && Player.ManaPercent <= Settings["use.on_mana_percent"].Cast<Slider>().CurrentValue
             || Player.HealthPercent <= (Settings["use.on_health_percent"].Cast<Slider>().CurrentValue / 2)
             || Player.ManaPercent <= (Settings["use.on_mana_percent"].Cast<Slider>().CurrentValue / 2))
            {
                Item.UseItem(Flask);
            }
        }

        public static bool InFountain(Obj_AI_Base champ)
        {
            var fountainRange = 1050;
            return champ.IsVisible && ObjectManager.Get<Obj_SpawnPoint>().Any(sp => champ.Distance(sp.Position) < fountainRange);
        }
    }
}
