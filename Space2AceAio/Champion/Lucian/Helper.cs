﻿using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace Space2Ace_Lucian
{
    internal class Helper
    {
        public static void LucianAntiGapcloser(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (sender.IsEnemy && spell.End.LSDistance(ObjectManager.Player.Position) < LucianSpells.E.Range &&
                !spell.SData.IsAutoAttack() && spell.Target.IsMe)
            {
                foreach (
                    var gapclose in
                        AntiGapcloseSpell.GapcloseableSpells.Where(
                            x => spell.SData.Name == ((AIHeroClient) sender).GetSpell(x.Slot).Name)
                            .OrderByDescending(
                                c => Program.getSliderItem(Program.miscMenu, "gapclose.slider." + spell.SData.Name)))
                {
                    if (Program.getCheckBoxItem(Program.miscMenu, "gapclose." + ((AIHeroClient) sender).ChampionName))
                    {
                        LucianSpells.E.Cast(ObjectManager.Player.Position.LSExtend(spell.End, -LucianSpells.W.Range));
                    }
                }
            }
        }
    }
}