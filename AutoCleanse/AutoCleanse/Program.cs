using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCleanse
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    // Make menu

    class Program
    {
        private SpellSlot cleanse { get; set; }
        private const string MenuName = "AutoChat";
        public static Menu Settings;
        
        int QSS = (int)3140;
        int Dervish = (int)3137;
        int Mikaels = (int)3222;
        int Mercurial = (int)3139;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        public Obj_AI_Base Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        void OnLoad(EventArgs args)
        {
            CreateMenu();
            Game.OnUpdate += OnUpdate;
        }

        static void CreateMenu()
        {
            Settings = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Settings.AddGroupLabel("AutoCleanse Settings");
            Settings.Add("use.cleanse", new CheckBox("Use Cleanse"));
            Settings.Add("use.cleansers", new CheckBox("Use Cleansers"));
            Settings.Add("use.cleansevsignite", new CheckBox("Cleanse enemy Ignite"));
            Settings.Add("panic_key_enable", new CheckBox("Only Cleanse when pressed button enable", false));
            Settings.Add("use.panic_key", new KeyBind("Only Cleanse when pressed button", 1, "Press", ));
            Settings.Add("use.cleansers.second.priority", new CheckBox("Use Cleansers for second-priority ultimates", false));
            Settings.Add("use.delay", new Slider("Delay cleanse/cleansers usage by X ms", 500, 0, 2000));
        }

        bool HasNoProtection(Obj_AI_Base target)
        {
            // return "true" if the Player..
            return

                //..has no SpellShield..
                !target.HasBuffOfType(BuffType.SpellShield)

             //..nor SpellImmunity.  
             && !target.HasBuffOfType(BuffType.SpellImmunity)
            ;
        }

        bool ShouldUseCleanse(Obj_AI_Base target)
        {
            // return "true" if the Player is being affected by..
            return (
                // ..Charms..
                target.HasBuffOfType(BuffType.Charm)

             // ..or Fears..
             || target.HasBuffOfType(BuffType.Flee)

             // ..or Polymorphs..
             || target.HasBuffOfType(BuffType.Polymorph)

             // ..or Snares..
             || target.HasBuffOfType(BuffType.Snare)

             // ..or Stuns..
             || target.HasBuffOfType(BuffType.Stun)

             // ..or Taunts..
             || target.HasBuffOfType(BuffType.Taunt)

             // ..or Exhaust..
             || target.HasBuff("summonerexhaust")

            )
             //..and, if he has no protection..
             && HasNoProtection(target)

             // ..and the relative option is enabled.
             && Settings["use.cleanse"].Cast<CheckBox>().CurrentValue
            ;
        }

        bool ShouldUseCleanser()
        {
            // return "true" if the Player is being affected by..
            return (
                // ..Zed's Target Mark (R)..
                Player.HasBuff("ZedR")

             // ..or Vladimir's Mark (R)..
             || Player.HasBuff("VladimirHemoplague")

             // ..or Mordekaiser's Mark (R)..
             || Player.HasBuff("MordekaiserChildrenOfTheGrave")

             // ..or Poppy's Immunity Mark (R)..
             || Player.HasBuff("PoppyDiplomaticImmunity")

             // ..or Fizz's Fish Mark (R)..
             || Player.HasBuff("FizzMarinerDoom")

             // ..or Suppressions..
             || Player.HasBuffOfType(BuffType.Suppression)
            )
             //..and, if he has no protection..
             && HasNoProtection(Player)

             // ..and the relative option is enabled.
             && Settings["use.cleansers"].Cast<CheckBox>().CurrentValue
            ;
        }

        private void UseCleanser()
        {
            // if the player has QuickSilver Sash and is able to use it..
            if (Item.HasItem(QSS) && Item.CanUseItem(QSS))
            {
                // ..JUST (DO)USE IT!
                Item.UseItem(QSS);
            }

            // else if the player has Dervish Blade and is able to use it..
            else if (Item.HasItem(Dervish) && Item.CanUseItem(Dervish))
            {
                // ..JUST (DO)USE IT!
                Item.UseItem(Dervish);
            }

            // else if the player has Mikaels Crucible and is able to use it..
            else if (Item.HasItem(Mikaels) && Item.CanUseItem(Mikaels))
            {
                // ..JUST (DO)USE IT!
                Item.UseItem(Mikaels);
            }

            // else if the player has Mercurial Scimitar and is able to use it..
            else if (Item.HasItem(Mercurial) && Item.CanUseItem(Mercurial))
            {
                // ..JUST (DO)USE IT!
                Item.UseItem(Mercurial);
            }
        }

        bool ShouldUseSecondPriorityCleanser()
        {
            // return "true" if the Player has..
            return (
                // ..Twisted Fates vision mark (R)..
                Player.Buffs.Any(b => b.Name.Contains("Twisted"))

             // ..Nocturnes R (Fog part)..
             || Player.Buffs.Any(b => b.Name.Contains("Nocturne"))
            )

            //..and, if he has no protection..
            && HasNoProtection(Player)

            // ..and the relative option is enabled.
            && Settings["use.cleansers.second.priority"].Cast<CheckBox>().CurrentValue;
        }

        bool CanAndShouldCleanseIfIgnited()
        {
            // return "true" if..
            return
                // ..the player is ignited..
                Player.HasBuff("summonerdot")

            // ..and the relative option is enabled.
            && Settings["use.cleansevsignite"].Cast<CheckBox>().CurrentValue;
        }

        public void BuildMikaelsMenu(Menu Menu)
        {
            MikaelsMenu = new Menu("use.mikaelsmenu", "Mikaels Options", true);
            {
                foreach (var ally in ObjectManager.Get<Obj_AI_Base>()
                    .Where(h => h.IsAlly)
                    .Select(hero => hero.ChampionName)
                    .ToList())
                {
                    MikaelsMenu.Add(new MenuBool(string.Format("use.mikaels.{0}", ally.ToLowerInvariant()), ally, true));
                }
                MikaelsMenu.Add(new MenuBool("enable.mikaels", "Enable Mikaels Usage", true));
            }
        }

        /// <summary>
        ///    Called when the game updates itself.
        /// </summary>
        /// <param name="args">
        ///    The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void OnUpdate(EventArgs args)
        {
            // Don't use the assembly if the player is dead.
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            // If the only-cleanse-if-key-pressed option is enabled and the relative key is being pressed or the only-cleanse-if-key-pressed option is disabled..
            if ((Settings["panic_key_enable"].Cast<CheckBox>().CurrentValue && Settings["use.panic_key"].Cast<KeyBind>().CurrentValue)
                || (!Settings["panic_key_enable"].Cast<CheckBox>().CurrentValue))
            {
                cleanse = Player.GetSpellSlot("summonerboost");
                var CleanseDelay = Settings["use.delay"]..Cast<Slider>().CurrentValue;
                var IsCleanseReady = Player.Spellbook.CanUseSpell(cleanse) == (SpellState)12;

                // For each ally enabled on the menu-option..
                foreach (var ally in ObjectManager.Get<Obj_AI_Base>()
                    .Where(h => h.IsAlly
                        && Settings[string.Format("use.mikaels.{0}", h.ChampionName.ToLowerInvariant())].Cast<CheckBox>().CurrentValue
                        && Settings["enable.mikaels"].Cast<CheckBox>().CurrentValue))
                {
                    // if the player has Mikaels and is able to use it..
                    if (Item.HasItem(Mikaels) && Item.CanUseItem(Mikaels))
                    {
                        // If the ally should be cleansed..
                        if (ShouldUseCleanse(ally))
                        {
                            // ..JUST (DO)CLEANSE HIM!
                            Core.DelayAction(CleanseDelay,
                                () =>
                                {
                                    Items.UseItem(Mikaels, ally);
                                    return;
                                }
                            );
                        }
                    }
                }

                // If you are being affected by movement-empairing or control-denying cctype or you are being affected by summoner Ignite..
                if (ShouldUseCleanse(Player) || CanAndShouldCleanseIfIgnited())
                {
                    // If the player actually has the summonerspell Cleanse and it is ready to use..
                    if (cleanse != SpellSlot.Unknown && IsCleanseReady)
                    {
                        // ..JUST (DO)CLEANSE IT!
                        Core.DelayAction(CleanseDelay,
                            () =>
                            {
                                Player.Spellbook.CastSpell(cleanse, Player);
                                return;
                            }
                        );
                    }
                }

                // If the player is being affected by Hard CC..
                if (ShouldUseCleanser() || ShouldUseSecondPriorityCleanser())
                {
                    // If the player is being affected by the DeathMark..
                    if (Player.HasBuff("zedulttargetmark"))
                    {
                        // ..Cleanse it, but delay the action by 1,5 seconds.
                        Core.DelayAction(() => { UseCleanser(); return; }, 1500);
                    }

                    // if the player has Mikaels and is able to use it..
                    if (Item.HasItem(Mikaels) && Item.CanUseItem(Mikaels))
                    {
                        if (ShouldUseCleanse(Player))
                        {
                            Core.DelayAction(CleanseDelay,
                                () =>
                                {
                                    Items.UseItem(Mikaels);
                                    return;
                                }
                            );
                        }
                    }

                    // ..JUST (DO)CLEANSE IT!
                    Core.DelayAction(CleanseDelay,
                        () =>
                        {
                            UseCleanser();
                            return;
                        }
                    );
                }

                // If the player has not cleanse or cleanse is on cooldown and the player is being affected by hard CC..
                if ((cleanse == SpellSlot.Unknown || !IsCleanseReady) && ShouldUseCleanse(Player))
                {
                    // ..JUST (DO)CLEANSE IT!
                    Core.DelayAction(CleanseDelay,
                        () =>
                        {
                            UseCleanser();
                            return;
                        }
                    );
                }
            }
        }
    }
}
