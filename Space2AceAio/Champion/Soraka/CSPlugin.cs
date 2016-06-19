
using System;
using System.Collections.Generic;
using System.Linq;
using Space2Ace_Soraka.Utils;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Data.Enumerations;

namespace Space2Ace_Soraka
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp.SDK.Core.Utils;

    public abstract class CSPlugin
    {
        public Menu CrossAssemblySettings;
        public bool IsPerformanceChallengerEnabled;
        public int TriggerOnUpdate;
        private int _lastOnUpdateTriggerT = 0;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }


        public CSPlugin()
        {
            MainMenu = EloBuddy.SDK.Menu.MainMenu.AddMenu("Space2Ace Full Automatic Soraka - PLAY WITHOUT SPACEBAR!", ObjectManager.Player.ChampionName);
            CrossAssemblySettings = MainMenu.AddSubMenu("Performance Booster");
            CrossAssemblySettings.Add("Space2Ace Performancer", new CheckBox("Use Space2Ace Performancer", false));
            CrossAssemblySettings.Add("triggeronupdate", new Slider("Trigger OnUpdate X times a second", 26, 20, 33));

            IsPerformanceChallengerEnabled = getCheckBoxItem(CrossAssemblySettings, "Space2Ace Performancer");
            TriggerOnUpdate = getSliderItem(CrossAssemblySettings, "triggeronupdate");

            Game.OnUpdate += this.DelayOnUpdate;
        }

        public IEnumerable<AIHeroClient> ValidTargets { get { return EntityManager.Heroes.Enemies.Where(enemy => enemy.IsHPBarRendered); } }
        public Menu MainMenu { get; set; }
        public virtual void OnUpdate(EventArgs args) { }
        public virtual void OnProcessSpellCast(GameObject sender, GameObjectProcessSpellCastEventArgs args) { }
        public virtual void OnDraw(EventArgs args) { }
        public virtual void InitializeMenu() { }

        public delegate void DelayedOnUpdateEH(EventArgs args);

        public event DelayedOnUpdateEH DelayedOnUpdate;

        public void DelayOnUpdate(EventArgs args)
        {
            IsPerformanceChallengerEnabled = getCheckBoxItem(CrossAssemblySettings, "Space2Ace Performancer");
            TriggerOnUpdate = getSliderItem(CrossAssemblySettings, "triggeronupdate");
            if (this.DelayedOnUpdate != null)
            {
                if (this.IsPerformanceChallengerEnabled && Variables.TickCount - this._lastOnUpdateTriggerT > 1000 / this.TriggerOnUpdate)
                {
                    this._lastOnUpdateTriggerT = Variables.TickCount;
                    this.DelayedOnUpdate(args);
                    return;
                }
                this.DelayedOnUpdate(args);
            }
        }
    }
}