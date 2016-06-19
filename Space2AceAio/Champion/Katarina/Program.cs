using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Microsoft.Win32;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace Space2Ace_Katarina
{
    class Program
    {

        #region Declaration

        private static bool ShallJumpNow;
        private static Vector3 JumpPosition;
        private static LeagueSharp.Common.Spell Q, W, E, R;
        private static Menu _menu;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static AIHeroClient qTarget;
        private static Obj_AI_Base qMinion;
        private static readonly AIHeroClient[] AllEnemy = HeroManager.Enemies.ToArray();
        private static bool WardJumpReady;
        private static SpellSlot IgniteSpellSlot = SpellSlot.Unknown;
        private static readonly List<int> AllEnemyTurret = new List<int>();
        private static readonly List<int> AllAllyTurret = new List<int>();
        private static Dictionary<int, bool> TurretHasAggro = new Dictionary<int, bool>();
        private static int lastLeeQTick;
        private static int tickValue;

        #endregion

        static bool IsTurretPosition(Vector3 pos)
        {
            float mindistance = 2000;
            foreach (int NetID in AllEnemyTurret)
            {
                Obj_AI_Turret turret = ObjectManager.GetUnitByNetworkId<Obj_AI_Turret>((uint)NetID);
                if (turret != null && !turret.IsDead && !TurretHasAggro[NetID])
                {
                    float distance = pos.LSDistance(turret.Position);
                    if (mindistance >= distance)
                    {
                        mindistance = distance;

                    }

                }
            }
            return mindistance <= 950;
        }

        public static Menu comboMenu, harassMenu, laneclear, jungleclear, lasthit, ksMenu, drawingsMenu, miscMenu, performanceMenu, devMenu;

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

        public static void Game_OnGameLoad()
        {
            //Wird aufgerufen, wenn LeagueSharp Injected
            if (Player.ChampionName != "Katarina")
            {
                return;
            }
            #region Spells
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 675, DamageType.Magical);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 375, DamageType.Magical);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 700, DamageType.Magical);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 550, DamageType.Magical);
            //Get Ignite
            if (Player.Spellbook.GetSpell(SpellSlot.Summoner1).Name.Contains("summonerdot"))
            {
                IgniteSpellSlot = SpellSlot.Summoner1;
            }
            if (Player.Spellbook.GetSpell(SpellSlot.Summoner2).Name.Contains("summonerdot"))
            {
                IgniteSpellSlot = SpellSlot.Summoner2;
            }
            #endregion

            foreach (Obj_AI_Turret turret in ObjectManager.Get<Obj_AI_Turret>())
            {
                if (turret.IsEnemy)
                {
                    AllEnemyTurret.Add(turret.NetworkId);
                    TurretHasAggro[turret.NetworkId] = false;
                }
                if (turret.IsAlly)
                {
                    AllAllyTurret.Add(turret.NetworkId);
                    TurretHasAggro[turret.NetworkId] = false;
                }
            }

            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = CalculateDamage;


            #region Menu
            _menu = MainMenu.AddMenu("Space2Ace Katarina", "Space2Ace Katarina");

            //Combo-Menü
            comboMenu = _menu.AddSubMenu("Combo", "Space2Ace_Katarina.combo");
            comboMenu.Add("Space2Ace_Katarina.combo.useq", new CheckBox("Use Q"));
            comboMenu.Add("Space2Ace_Katarina.combo.usew", new CheckBox("Use W"));
            comboMenu.Add("Space2Ace_Katarina.combo.usee", new CheckBox("Use E"));
            comboMenu.Add("Space2Ace_Katarina.combo.user", new CheckBox("Use R"));
            comboMenu.Add("Space2Ace_Katarina.combo.mode", new ComboBox("Combo mode", 0, "Standard Space2Ace Combo", "MATRIX MODE DANGEROUS !!!"));
            comboMenu.Add("Space2Ace_Katarina.combo.order", new ComboBox("Rotation Order", 0, "Single Target Focused", "AOE Target Focused", "Dynamic"));

            //Harrass-Menü
            harassMenu = _menu.AddSubMenu("Harass", "Space2Ace_Katarina.harrass");
            harassMenu.Add("Space2Ace_Katarina.harass.useq", new CheckBox("Use Q"));
            harassMenu.Add("Space2Ace_Katarina.harass.usew", new CheckBox("Use W"));
            harassMenu.AddGroupLabel("Auto-Harass");
            harassMenu.Add("Space2Ace_Katarina.harass.autoharass.toggle", new CheckBox("Automatic Harrass"));
            harassMenu.Add("Space2Ace_Katarina.harass.autoharass.key", new KeyBind("Toogle Harrass", false, KeyBind.BindTypes.PressToggle, 'N'));
            harassMenu.Add("Space2Ace_Katarina.harass.autoharass.useq", new CheckBox("Use Q", false));
            harassMenu.Add("Space2Ace_Katarina.harass.autoharass.usew", new CheckBox("Use W"));

            //Laneclear-Menü
            laneclear = _menu.AddSubMenu("Laneclear", "Space2Ace_Katarina.laneclear");
            laneclear.Add("Space2Ace_Katarina.laneclear.useq", new CheckBox("Use Q"));
            laneclear.Add("Space2Ace_Katarina.laneclear.usew", new CheckBox("Use W"));
            laneclear.Add("Space2Ace_Katarina.laneclear.minw", new Slider("Minimum Minions to use W", 3, 1, 6));
            laneclear.Add("Space2Ace_Katarina.laneclear.minwlasthit", new Slider("Minimum Minions to Lasthit with W", 2, 0, 6));

            //Jungleclear-Menü
            jungleclear = _menu.AddSubMenu("Jungleclear", "Space2Ace_Katarina.jungleclear");
            jungleclear.Add("Space2Ace_Katarina.jungleclear.useq", new CheckBox("Use Q"));
            jungleclear.Add("Space2Ace_Katarina.jungleclear.usew", new CheckBox("Use W"));
            jungleclear.Add("Space2Ace_Katarina.jungleclear.usee", new CheckBox("Use E"));

            //Lasthit-Menü
            lasthit = _menu.AddSubMenu("Lasthit", "Space2Ace_Katarina.lasthit");
            lasthit.Add("Space2Ace_Katarina.lasthit.useq", new CheckBox("Use Q"));
            lasthit.Add("Space2Ace_Katarina.lasthit.usew", new CheckBox("Use W"));
            lasthit.Add("Space2Ace_Katarina.lasthit.usee", new CheckBox("Use E", false));
            lasthit.Add("Space2Ace_Katarina.lasthit.noenemiese", new CheckBox("Only use E when no Enemies are around"));

            //KS-Menü
            ksMenu = _menu.AddSubMenu("Killsteal", "Space2Ace_Katarina.killsteal");
            ksMenu.Add("Space2Ace_Katarina.killsteal.useq", new CheckBox("Use Q"));
            ksMenu.Add("Space2Ace_atarina.killsteal.usew", new CheckBox("Use W"));
            ksMenu.Add("Space2Ace_Katarina.killsteal.usee", new CheckBox("Use E"));
            ksMenu.Add("Space2Ace_Katarina.killsteal.usef", new CheckBox("Use Ignite"));
            ksMenu.Add("Space2Ace_Katarina.killsteal.wardjump", new CheckBox("KS with Wardjump"));

            //Drawings-Menü
            drawingsMenu = _menu.AddSubMenu("Drawings", "Space2Ace_Katarina.drawings");
            drawingsMenu.Add("Space2Ace_Katarina.drawings.drawq", new CheckBox("Draw Q", false));
            drawingsMenu.Add("Space2Ace_Katarina.drawings.draww", new CheckBox("Draw W", false));
            drawingsMenu.Add("Space2Ace_Katarina.drawings.drawe", new CheckBox("Draw E", false));
            drawingsMenu.Add("Space2Ace_Katarina.drawings.drawr", new CheckBox("Draw R", false));
            drawingsMenu.Add("Space2Ace_Katarina.drawings.dmg", new CheckBox("Draw Damage to target"));
            drawingsMenu.Add("Space2Ace_Katarina.drawings.drawalways", new CheckBox("Draw Always", false));

            //Misc-Menü
            miscMenu = _menu.AddSubMenu("Miscellanious", "Space2Ace_Katarina.misc");
            miscMenu.Add("Space2Ace_Katarina.misc.wardjump", new CheckBox("Use Wardjump"));
            miscMenu.Add("Space2Ace_Katarina.misc.wardjumpkey", new KeyBind("Wardjump Key", false, KeyBind.BindTypes.HoldActive, 'Z'));
            miscMenu.Add("Space2Ace_Katarina.misc.noRCancel", new CheckBox("Prevent R Cancel"));
            miscMenu.Add("Space2Ace_Katarina.misc.cancelR", new CheckBox("Cancel R when no Enemies are around", false));
            miscMenu.Add("Space2Ace_Katarina.misc.kswhileult", new CheckBox("Do Killsteal while Ulting"));
            miscMenu.Add("Space2Ace_Katarina.misc.allyTurret", new CheckBox("Jump unter Turret for Gapcloser"));

            performanceMenu = _menu.AddSubMenu("Performance", "Space2Ace_Katarina.performance");
            performanceMenu.Add("Space2Ace_Katarina.performance.track", new Slider("Tracked minions for Lasthitting", 3, 1, 10));
            performanceMenu.Add("Space2Ace_Katarina.performance.tickmanager", new CheckBox("Enable Tickmanager", false));
            performanceMenu.Add("Space2Ace_Katarina.performance.ticks", new Slider("Update Frequency for Tickmanager", 8, 2, 50));

            //Dev-Menü
            devMenu = _menu.AddSubMenu("Dev", "Space2Ace_Katarina.dev");
            devMenu.Add("Space2Ace_Katarina.dev.enable", new CheckBox("Enable Dev-Tools", false));
            devMenu.Add("Space2Ace_katarina.dev.targetdistance", new KeyBind("Distance to Target", false, KeyBind.BindTypes.HoldActive, 'L'));

            //alles zum Hauptmenü hinzufügen

            #endregion
          
            #region Subscriptions
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Obj_AI_Turret.OnBasicAttack += Turret_OnTarget;
            Obj_AI_Base.OnBuffLose += BuffRemove;


            #endregion
        }



        private static void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawingsMenu, "Space2Ace_Katarina.drawings.drawq") && (Q.IsReady() || getCheckBoxItem(drawingsMenu, "Space2Ace_Katarina.drawings.drawalways")))
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.IndianRed);
            if (getCheckBoxItem(drawingsMenu, "Space2Ace_Katarina.drawings.draww") && (W.IsReady() || getCheckBoxItem(drawingsMenu, "Space2Ace_Katarina.drawings.drawalways")))
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.IndianRed);
            if (getCheckBoxItem(drawingsMenu, "Space2Ace_Katarina.drawings.drawe") && (E.IsReady() || getCheckBoxItem(drawingsMenu, "Space2Ace_Katarina.drawings.drawalways")))
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.IndianRed);
            if (getCheckBoxItem(drawingsMenu, "Space2Ace_Katarina.drawings.drawr") && (R.IsReady() || getCheckBoxItem(drawingsMenu, "Space2Ace_Katarina.drawings.drawalways")))
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.IndianRed);
        }


        private static void BuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name == "BlindMonkQOne")
            {
                lastLeeQTick = Utils.TickCount;
            }
        }


        static void Game_OnUpdate(EventArgs args)
        {
            Demark();
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = getCheckBoxItem(drawingsMenu, "Space2Ace_katarina.drawings.dmg");
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }
            if (HasRBuff())
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                if (getCheckBoxItem(miscMenu, "Space2Ace_Katarina.misc.cancelR") && Player.GetEnemiesInRange(R.Range + 50).Count == 0)
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                if (getCheckBoxItem(miscMenu, "Space2Ace_Katarina.misc.kswhileult"))
                    Killsteal();
                return;
            }
            if (ShallJumpNow)
            {
                WardJump(JumpPosition, false, false);
                if (!E.IsReady())
                {
                    ShallJumpNow = false;
                }
            }
            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;

            //Dev();
            Killsteal();

            //Combo
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Combo(Q.IsReady() && getCheckBoxItem(comboMenu, "Space2Ace_Katarina.combo.useq"), W.IsReady() && getCheckBoxItem(comboMenu, "Space2Ace_Katarina.combo.usew"), E.IsReady() && getCheckBoxItem(comboMenu, "Space2Ace_Katarina.combo.usee"), R.IsReady() && getCheckBoxItem(comboMenu, "Space2Ace_Katarina.combo.user"));

            //Harass
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                Combo(Q.IsReady() && getCheckBoxItem(harassMenu, "Space2Ace_Katarina.harass.useq"), W.IsReady() && getCheckBoxItem(harassMenu, "Space2Ace_Katarina.harass.usew"), false, false, true);

            //Autoharass
            if (getCheckBoxItem(harassMenu, "Space2Ace_Katarina.harass.autoharass.toggle") && getKeyBindItem(harassMenu, "Space2Ace_Katarina.harass.autoharass.key"))
                Combo(Q.IsReady() && getCheckBoxItem(harassMenu, "Space2Ace_Katarina.harass.autoharass.useq"), W.IsReady() && getCheckBoxItem(harassMenu, "Space2Ace_Katarina.harass.autoharass.usew"), false, false, true);

            Lasthit();
            LaneClear();
            JungleClear();

            if (getKeyBindItem(miscMenu, "Space2Ace_Katarina.misc.wardjumpkey") && getCheckBoxItem(miscMenu, "Space2Ace_Katarina.misc.wardjump"))
            {
                WardJump(Game.CursorPos);
            }
        }

        private static void Dev()
        {
            if (getCheckBoxItem(devMenu, "Space2Ace_Katarina.dev.enable") && getKeyBindItem(devMenu, "Space2Ace_Katarina.dev.targetdistance"))
            {
                AIHeroClient target = TargetSelector.GetTarget(1000, DamageType.Magical);
                if (target != null)
                {
                    Chat.Print("Distance to Target:" + Player.LSDistance(target));
                }
            }
        }


        static bool HasRBuff()
        {
            return Player.HasBuff("KatarinaR") || Player.IsChannelingImportantSpell() || Player.HasBuff("katarinarsound");

        }

        static void Combo(bool useq, bool usew, bool usee, bool user, bool anyTarget = false)
        {
            bool startWithQ = getBoxItem(comboMenu, "Space2Ace_Katarina.combo.order") == 0 && useq;
            bool dynamic = getBoxItem(comboMenu, "Space2Ace_Katarina.combo.order") == 2;
            bool smartcombo = getBoxItem(comboMenu, "Space2Ace_Katarina.combo.mode") == 0;
            AIHeroClient target = TargetSelector.GetTarget(!startWithQ || dynamic ? E.Range : Q.Range, DamageType.Magical);
            if (target != null && !target.IsZombie)
            {
                if (useq && (startWithQ || !usee || dynamic) && target.LSDistance(Player) < Q.Range)
                {
                    Q.Cast(target);
                    qTarget = target;
                    return;
                }
                if (usee && (usew || user || qTarget != target || !smartcombo))
                {
                    E.Cast(target);
                    return;
                }
                if (anyTarget)
                {
                    List<AIHeroClient> enemies = Player.Position.GetEnemiesInRange(390);
                    if (enemies.Count >= 2)
                    {
                        W.Cast();
                        return;
                    }
                    if (enemies.Count == 1)
                    {
                        target = enemies.ElementAt(0);
                    }
                }
                if (target.LSDistance(Player) < 390 && usew && (user || qTarget != target || !smartcombo))
                {
                    W.Cast();
                    return;
                }
                if (target.LSDistance(Player) < R.Range - 200 && user)
                {
                    R.Cast();
                }
            }
        }

        private static void Turret_OnTarget(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.GetType() == typeof(Obj_AI_Turret))
            {
                TurretHasAggro[sender.NetworkId] = !(args.Target == null || args.Target is Obj_AI_Minion);
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "KatarinaQ" && args.Target.GetType() == typeof(AIHeroClient))
            {
                qTarget = (AIHeroClient)args.Target;
            }
            if (args.SData.Name == "katarinaE")
            {
                WardJumpReady = false;
            }
            if (sender.IsMe && WardJumpReady)
            {
                E.Cast((Obj_AI_Base)args.Target);
                WardJumpReady = false;
            }
            //Todo Check for Lee Q
            if (args.SData.Name == "blindmonkqtwo")
            {

                if (lastLeeQTick - Utils.TickCount <= 10)
                {
                    //Game.PrintChat("Trying to Jump undeder Ally Turret - OnProcessSpellCast");
                    JumpUnderTurret(-100, sender.Position);
                }
                lastLeeQTick = Utils.TickCount;
            }
            // Todo Test
            if (args.Target != null && args.Target.IsMe && getCheckBoxItem(miscMenu, "Space2Ace_Katarina.misc.allyTurret"))
            {
                switch (args.SData.Name)
                {
                    case "ZedR":
                        JumpUnderTurret(-100, sender.Position);
                        break;
                    case "ViR":
                        JumpUnderTurret(100, sender.Position);
                        break;
                    case "NocturneParanoia":
                        JumpUnderTurret(100, sender.Position);
                        break;
                    case "MaokaiUnstableGrowth":
                        JumpUnderTurret(0, sender.Position);
                        break;
                }

            }

        }



        private static void JumpUnderTurret(float extrarange, Vector3 objectPosition)
        {
            float mindistance = 100000;
            //Getting next Turret
            Obj_AI_Turret turretToJump = null;

            foreach (int NetID in AllAllyTurret)
            {
                Obj_AI_Turret turret = ObjectManager.GetUnitByNetworkId<Obj_AI_Turret>((uint)NetID);
                if (turret != null && !turret.IsDead)
                {
                    float distance = Player.Position.LSDistance(turret.Position);
                    if (mindistance >= distance)
                    {
                        mindistance = distance;
                        turretToJump = turret;
                    }

                }
            }
            if (turretToJump != null && !TurretHasAggro[turretToJump.NetworkId] && Player.Position.LSDistance(turretToJump.Position) < 1500)
            {
                int i = 0;

                do
                {
                    Vector3 extPos = Player.Position.LSExtend(turretToJump.Position, 685 - i);
                    float dist = objectPosition.LSDistance(extPos + extrarange);
                    Vector3 predictedPosition = objectPosition.LSExtend(extPos, dist);
                    if (predictedPosition.LSDistance(turretToJump.Position) <= 890 && !predictedPosition.IsWall())
                    {
                        WardJump(Player.Position.LSExtend(turretToJump.Position, 650 - i), false);
                        JumpPosition = Player.Position.LSExtend(turretToJump.Position, 650 - i);
                        ShallJumpNow = true;
                        break;
                    }

                    i += 50;
                } while (i <= 300 || !Player.Position.Extend(turretToJump.Position, 650 - i).IsWall());
            }

        }


        static void Demark()
        {
            if ((qTarget != null && qTarget.HasBuff("katarinaqmark")) || Q.Cooldown < 3)
            {
                qTarget = null;
            }
        }


        #region WardJumping
        private static void WardJump(Vector3 where, bool move = true, bool placeward = true)
        {
            if (move)
                Orbwalker.MoveTo(Game.CursorPos);
            if (!E.IsReady())
            {
                return;
            }
            Vector3 wardJumpPosition = Player.Position.LSDistance(where) < 600 ? where : Player.Position.LSExtend(where, 600);
            var lstGameObjects = ObjectManager.Get<Obj_AI_Base>().ToArray();
            Obj_AI_Base entityToWardJump = lstGameObjects.FirstOrDefault(obj =>
                obj.Position.LSDistance(wardJumpPosition) < 150
                && (obj is Obj_AI_Minion || obj is AIHeroClient)
                && !obj.IsMe && !obj.IsDead
                && obj.Position.LSDistance(Player.Position) < E.Range);

            if (entityToWardJump != null)
            {
                E.Cast(entityToWardJump);
            }
            else if (placeward)
            {
                int wardId = GetWardItem();
                if (wardId != -1 && !wardJumpPosition.IsWall())
                {
                    WardJumpReady = true;
                    PutWard(wardJumpPosition.To2D(), (ItemId)wardId);
                }
            }

        }

        public static int GetWardItem()
        {
            int[] wardItems = { 3340, 3350, 3205, 3207, 2049, 2045, 2044, 3361, 3154, 3362, 3160, 2043 };
            foreach (var id in wardItems.Where(id => Items.HasItem(id) && Items.CanUseItem(id)))
                return id;
            return -1;
        }

        public static void PutWard(Vector2 pos, ItemId warditem)
        {

            foreach (var slot in Player.InventoryItems.Where(slot => slot.Id == warditem))
            {
                ObjectManager.Player.Spellbook.CastSpell(slot.SpellSlot, pos.To3D());
                return;
            }
        }
        #endregion
        //Calculating Damage
        static float CalculateDamage(AIHeroClient target)
        {
            double damage = 0d;
            if (Q.IsReady())
            {
                damage += Player.GetSpellDamage(target, SpellSlot.Q) + Player.LSGetSpellDamage(target, SpellSlot.Q, 1);
            }
            if (target.HasBuff("katarinaqmark") || target == qTarget)
            {
                damage += Player.LSGetSpellDamage(target, SpellSlot.Q, 1);
            }
            if (W.IsReady())
            {
                damage += Player.GetSpellDamage(target, SpellSlot.W);
            }
            if (E.IsReady())
            {
                damage += Player.GetSpellDamage(target, SpellSlot.E);
            }
            if (R.IsReady() || (Player.GetSpell(R.Slot).State == SpellState.Surpressed && R.Level > 0))
            {
                damage += Player.GetSpellDamage(target, SpellSlot.R);
            }
            if (Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite) > 0 && IgniteSpellSlot != SpellSlot.Unknown && IgniteSpellSlot.IsReady())
            {
                damage += Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
                damage -= target.HPRegenRate * 2.5;
            }
            return (float)damage;
        }

        #region Killsteal
        static int CanKill(AIHeroClient target, bool useq, bool usew, bool usee, bool usef)
        {
            double damage = 0;
            if (!useq && !usew && !usee && !usef)
                return 0;
            if (Q.IsReady() && useq)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
                if ((W.IsReady() && usew) || (E.IsReady() && usee))
                {
                    damage += ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q, 1);
                }
            }
            if (target.HasBuff("katarinaqmark") || target == qTarget)
            {
                damage += ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.Q, 1);
            }
            if (W.IsReady() && usew)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
            }
            if (E.IsReady() && usee)
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            }
            if (damage >= target.Health)
            {
                return 1;
            }
            if (Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite) > 0 && !target.HasBuff("summonerdot") && !HasRBuff() && IgniteSpellSlot != SpellSlot.Unknown && IgniteSpellSlot.IsReady())
            {
                damage += Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
                damage -= target.HPRegenRate * 2.5;
            }
            return damage >= target.Health ? 2 : 0;

        }

        private static void Killsteal()
        {
            foreach (AIHeroClient enemy in AllEnemy)
            {
                if (enemy == null || enemy.HasBuffOfType(BuffType.Invulnerability))
                    return;

                if (CanKill(enemy, false, getCheckBoxItem(ksMenu, "Space2Ace_Katarina.killsteal.usew"), false, false) == 1 && enemy.IsValidTarget(390))
                {
                    W.Cast(enemy);
                    return;
                }
                if (CanKill(enemy, false, false, getCheckBoxItem(ksMenu, "Space2Ace_Katarina.killsteal.usee"), false) == 1 && enemy.IsValidTarget(700))
                {
                    E.Cast(enemy);
                    return;
                }
                if (CanKill(enemy, getCheckBoxItem(ksMenu, "Space2Ace_Katarina.killsteal.useq"), false, false, false) == 1 && enemy.IsValidTarget(675))
                {
                    Q.Cast(enemy);
                    qTarget = enemy;
                    return;
                }
                int cankill = CanKill(enemy, getCheckBoxItem(ksMenu, "Space2Ace_atarina.killsteal.useq"), getCheckBoxItem(ksMenu, "Space2Ace_Katarina.killsteal.usew"), getCheckBoxItem(ksMenu, "Space2Ace_Katarina.killsteal.usee"), getCheckBoxItem(ksMenu, "Space2Ace_Katarina.killsteal.usef"));
                if ((cankill == 1 || cankill == 2) && enemy.IsValidTarget(Q.Range))
                {
                    if (cankill == 2 && enemy.IsValidTarget(600))
                        Player.Spellbook.CastSpell(IgniteSpellSlot, enemy);
                    if (Q.IsReady())
                        Q.Cast(enemy);
                    if (E.IsReady() && (W.IsReady() || qTarget != enemy))
                        E.Cast(enemy);
                    if (W.IsReady() && enemy.IsValidTarget(390) && qTarget != enemy)
                        W.Cast();
                    return;
                }
                //KS with Wardjump
                cankill = CanKill(enemy, true, false, false, getCheckBoxItem(ksMenu, "Space2Ace_Katarina.killsteal.usef"));
                if (getCheckBoxItem(ksMenu, "Space2Ace_Katarina.killsteal.wardjump") && (cankill == 1 || cankill == 2) && enemy.IsValidTarget(1300) && Q.IsReady() && E.IsReady())
                {
                    WardJump(enemy.Position, false);
                    if (cankill == 2 && enemy.IsValidTarget(600))
                        Player.Spellbook.CastSpell(IgniteSpellSlot, enemy);
                    if (enemy.IsValidTarget(675))
                        Q.Cast(enemy);
                    return;
                }
            }
        }
        #endregion



        #region Lasthit

        private static void Lasthit()
        {

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) || (getCheckBoxItem(performanceMenu, "Space2Ace_Katarina.performance.tickmanager") && Utils.TickCount < tickValue))
                return;
            Obj_AI_Base[] sourroundingMinions;
            int tickCount = getSliderItem(performanceMenu, "Space2Ace_Katarina.performance.ticks");
            tickValue = Utils.TickCount + tickCount;
            if (getCheckBoxItem(lasthit, "Space2Ace_Katarina.lasthit.usew") && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, 390).Take(3).ToArray();
                {
                    //Only Cast W when minion is not killable with Autoattacks
                    if (
                        sourroundingMinions.Any(
                            minion =>
                                !minion.IsDead && Orbwalker.LastTarget != minion && (qMinion == null || minion != qMinion) &&
                                W.GetDamage(minion) > minion.Health &&
                                HealthPrediction.GetHealthPrediction(minion,
                                    (Player.CanAttack
                                        ? Game.Ping / 2
                                        : Orbwalker.LastAutoAttack - Utils.GameTimeTickCount +
                                          (int)Player.AttackDelay * 1000) + 200 + (getCheckBoxItem(performanceMenu, "Space2Ace_Katarina.performance.tickmanager") ? tickCount - 1 : 0) + (int)Player.AttackCastDelay * 1000) <= 0))
                    {
                        W.Cast();
                    }
                }

            }
            if (getCheckBoxItem(lasthit, "Space2Ace_Katarina.lasthit.useq") && Q.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, Q.Range).ToArray();
                foreach (var minion in sourroundingMinions.Where(minion => !minion.IsDead && Q.GetDamage(minion) > minion.Health))
                {
                    Q.Cast(minion);
                    qMinion = minion;
                    break;
                }
            }
            if (getCheckBoxItem(lasthit, "Space2Ace_Katarina.lasthit.usee") && E.IsReady() && (!getCheckBoxItem(lasthit, "Space2Ace_Katarina.lasthit.noenemiese") || Player.GetEnemiesInRange(1000).Count == 0))
            {
                //Same Logic with W + not killable with W
                sourroundingMinions = MinionManager.GetMinions(Player.Position, E.Range).Take(getSliderItem(performanceMenu, "Space2Ace_Katarina.performance.track")).ToArray();
                {
                    foreach (var minions in sourroundingMinions.Where(
                        minion =>
                            !minion.IsDead && Orbwalker.LastTarget != minion && (qMinion == null || minion != qMinion) &&
                            E.GetDamage(minion) >= minion.Health &&
                            (!W.IsReady() || !getCheckBoxItem(lasthit, "Space2Ace_Katarina.lasthit.usew") || Player.Position.LSDistance(minion.Position) > 390)
                            &&
                            HealthPrediction.GetHealthPrediction(minion,
                                (Player.CanAttack
                                    ? Game.Ping / 2
                                    : Orbwalker.LastAutoAttack - Utils.GameTimeTickCount + (int)Player.AttackDelay * 1000) +
                                200 + (getCheckBoxItem(performanceMenu, "Space2Ace_Katarina.performance.tickmanager") ? tickCount - 1 : 0) + (int)Player.AttackCastDelay * 1000) <= 0
                            &&
                            !IsTurretPosition(Player.Position.LSExtend(minion.Position,
                                Player.Position.LSDistance(minion.Position) + 35))))
                    {
                        E.Cast(minions);
                        break;
                    }
                }
            }
        }
        #endregion

        #region LaneClear
        private static void LaneClear()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                return;
            Obj_AI_Base[] sourroundingMinions;
            if (getCheckBoxItem(laneclear, "Space2Ace_Katarina.laneclear.usew") && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, W.Range - 5).ToArray();
                if (sourroundingMinions.GetLength(0) >= getSliderItem(laneclear, "Space2Ace_Katarina.laneclear.minw"))
                {
                    int lasthittable = sourroundingMinions.Count(minion => W.GetDamage(minion) + (minion.HasBuff("katarinaqmark") ? Q.GetDamage(minion, 1) : 0) > minion.Health);
                    if (lasthittable >= getSliderItem(laneclear, "Space2Ace_Katarina.laneclear.minwlasthit"))
                    {
                        W.Cast();
                    }
                }
            }
            if (getCheckBoxItem(laneclear, "Space2Ace_Katarina.laneclear.useq") && Q.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, Q.Range - 5).ToArray();
                foreach (var minion in sourroundingMinions.Where(minion => !minion.IsDead))
                {
                    Q.Cast(minion);
                    break;
                }
            }
        }
        #endregion

        #region Jungleclear

        private static void JungleClear()
        {
            Obj_AI_Base[] sourroundingMinions;
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                return;
            if (getCheckBoxItem(jungleclear, "Space2Ace_Katarina.jungleclear.useq") && Q.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral).ToArray();
                float maxhealth = 0;
                int chosenminion = 0;
                if (sourroundingMinions.GetLength(0) >= 1)
                {
                    for (int i = 0; i < sourroundingMinions.Length; i++)
                    {
                        if (maxhealth < sourroundingMinions[i].MaxHealth)
                        {
                            maxhealth = sourroundingMinions[i].MaxHealth;
                            chosenminion = i;
                        }
                    }
                    Q.Cast(sourroundingMinions[chosenminion]);
                }
            }
            if (getCheckBoxItem(jungleclear, "Space2Ace_Katarina.jungleclear.usew") && W.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, W.Range - 5, MinionTypes.All, MinionTeam.Neutral).ToArray();
                if (sourroundingMinions.GetLength(0) >= 1)
                {
                    W.Cast();
                }
            }
            if (getCheckBoxItem(jungleclear, "Space2Ace_Katarina.jungleclear.usee") && E.IsReady())
            {
                sourroundingMinions = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.Neutral).ToArray();
                float maxhealth = 0;
                int chosenminion = 0;
                if (sourroundingMinions.GetLength(0) >= 1)
                {
                    for (int i = 0; i < sourroundingMinions.Length; i++)
                    {
                        if (maxhealth < sourroundingMinions[i].MaxHealth)
                        {
                            maxhealth = sourroundingMinions[i].MaxHealth;
                            chosenminion = i;
                        }
                    }
                    E.Cast(sourroundingMinions[chosenminion]);
                }
            }
        }
        #endregion
        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && HasRBuff() && getCheckBoxItem(miscMenu, "Space2Ace_Katarina.misc.noRCancel"))
                args.Process = false;
        }
    }
}