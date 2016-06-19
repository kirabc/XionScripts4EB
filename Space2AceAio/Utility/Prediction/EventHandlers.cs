﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Space2AcePrediction;
using XionCommon;
using EloBuddy;
using EloBuddy.SDK;
using SebbyLib;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Space2Ace_Prediction
{
    public class EventHandlers
    {
        private static bool[] handleEvent = { true, true, true, true };
        public static Menu menu;

        public static void Game_OnGameLoad(EventArgs args)
        {
            Prediction.Initialize();
            menu = Prediction.Config;
        }

        public static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                SpellSlot slot = ObjectManager.Player.GetSpellSlot(args.SData.Name);
                if (!XionCommon.Utility.IsValidSlot(slot))
                    return;

                if (!handleEvent[(int)slot])
                {
                    if (Prediction.Spells[(int)slot] != null)
                        handleEvent[(int)slot] = true;
                }
            }
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

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

        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                if (getCheckBoxItem(menu, "ENABLED") && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)))
                {
                    if (!XionCommon.Utility.IsValidSlot(args.Slot))
                        return;

                    if (Prediction.Spells[(int)args.Slot] == null)
                        return;

                    if (!getCheckBoxItem(menu, String.Format("{0}{1}", ObjectManager.Player.ChampionName, args.Slot)))
                        return;

                    if (handleEvent[(int)args.Slot])
                    {
                        args.Process = false;
                        handleEvent[(int)args.Slot] = false;
                        var enemy = args.EndPosition.GetEnemiesInRange(200f).OrderByDescending(p => XionCommon.Utility.GetPriority(p.ChampionName)).FirstOrDefault();
                        if (enemy == null)
                            enemy = TargetSelector.GetTarget(Prediction.Spells[(int)args.Slot].Range, DamageType.Physical);

                        if (enemy != null)
                        {
                            SebbyLib.Movement.SkillshotType CoreType2 = SebbyLib.Movement.SkillshotType.SkillshotLine;
                            bool aoe2 = false;

                            if (Prediction.Spells[(int)args.Slot].Type == SkillshotType.SkillshotCircle)
                            {
                                //CoreType2 = SebbyLib.Movement.SkillshotType.SkillshotCircle;
                                //aoe2 = true;
                            }

                            if (Prediction.Spells[(int)args.Slot].Width > 80 && !Prediction.Spells[(int)args.Slot].Collision)
                                aoe2 = true;

                            var predInput2 = new SebbyLib.Movement.PredictionInput
                            {
                                Aoe = aoe2,
                                Collision = Prediction.Spells[(int)args.Slot].Collision,
                                Speed = Prediction.Spells[(int)args.Slot].Speed,
                                Delay = Prediction.Spells[(int)args.Slot].Delay,
                                Range = Prediction.Spells[(int)args.Slot].Range,
                                From = Player.ServerPosition,
                                Radius = Prediction.Spells[(int)args.Slot].Width,
                                Unit = enemy,
                                Type = CoreType2
                            };
                            var poutput2 = SebbyLib.Movement.Prediction.GetPrediction(predInput2);

                            if (Prediction.Spells[(int)args.Slot].Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                                return;

                            if (getBoxItem(menu, "SPREDHITC") == 0) // Very High
                            {
                                if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.VeryHigh)
                                    Prediction.Spells[(int)args.Slot].Cast(poutput2.CastPosition);
                                else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= SebbyLib.Movement.HitChance.High)
                                {
                                    Prediction.Spells[(int)args.Slot].Cast(poutput2.CastPosition);
                                }

                            }
                            else if (getBoxItem(menu, "SPREDHITC") == 1)
                            {
                                if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.High)
                                    Prediction.Spells[(int)args.Slot].Cast(poutput2.CastPosition);

                            }
                            else if (getBoxItem(menu, "SPREDHITC") == 2)
                            {
                                if (poutput2.Hitchance >= SebbyLib.Movement.HitChance.Medium)
                                    Prediction.Spells[(int)args.Slot].Cast(poutput2.CastPosition);
                            }
                        }
                    }
                }
            }
        }
    }
}
