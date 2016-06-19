#region

using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Events;
using Space2Ace.Utility;
using LeagueSharp.Common;
using SharpDX;
using Space2Ace.Properties;

using EloBuddy.SDK;
using EloBuddy.SDK.Notifications;
// ReSharper disable ObjectCreationAsStatement

#endregion

namespace Space2Ace
{
    internal static class Init
    {
        private static void Main()
        {
            Loading.OnLoadingComplete += Initialize;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            //Console.WriteLine(Orbwalker.ActiveModesFlags.ToString());
        }

        private static Render.Sprite Intro;
        private static float IntroTimer = Game.Time;
        public static List<string> RandomUltChampsList = new List<string>(new[] { "Ezreal", "Jinx", "Ashe", "Draven", "Gangplank", "Ziggs", "Lux", "Xerath" });
        public static List<string> BaseUltList = new List<string>(new[] { "Jinx", "Ashe", "Draven", "Ezreal", "Karthus" });

        private static System.Drawing.Bitmap LoadImg(string imgName)
        {
            var bitmap = Resources.ResourceManager.GetObject(imgName) as System.Drawing.Bitmap;
            if (bitmap == null)
            {
                Console.WriteLine(imgName + ".png not found.");
            }
            return bitmap;
        }

        private static void Initialize(EventArgs args)
        {

            Notifications.Show(new SimpleNotification("Space2Ace", "Draven,Jinx,Kindred,Lucian,Lux,Ryze,Soraka,Vayne,Zed"), 20000);

            Loader.Menu();

            if (Loader.Prediction)

            {
                Space2Ace_Prediction.Prediction.Initialize();
                Chat.Print("Space2Ace - Prediction Loaded!");
            }


            if (Loader.Autoskiller)

            {
                Space2Ace_Autoskiller.Program.Game_OnGameLoad();
                Chat.Print("Space2Ace - Autoskiller Loaded!");
            }

            if (Loader.useTracker)
            {
                SpellTracker.Program.Game_OnGameLoad();
                Chat.Print("Space2Ace - Spelltracker Loaded!");
            }

            if (!Loader.champOnly)
            {
                if (Loader.useActivator)
                {
                    Xionivator.Entry.OnLoad();
                    Chat.Print("Space2Ace - Activator Loaded!");
                }

                if (Loader.useRecall)
                {
                    Space2Ace_RecallTracker.Program.Main();
                    Chat.Print("Space2Ace - Recalltracker Loaded!");
                }

                if (Loader.useSkin)
                {
                    SDK_SkinChanger.Program.Load();
                    Chat.Print("Space2Ace - SkinChanger Loaded!");
                }


                if (Loader.godTracker)
                {
                    GodJungleTracker.Program.OnGameLoad();
                    Chat.Print("Space2Ace - JungleTracker loaded!");
                }

                if (Loader.ping)
                {
                    new Space2Ace_PingTracker.Program();
                    Chat.Print("Space2Ace - PingTracker loaded!");
                }

                if (Loader.human)
                {
                    Humanizer.Program.Game_OnGameLoad();
                    Chat.Print("Space2Ace - Humanizer Loaded!");
                }

                if (Loader.gank)
                {
                    Space2Ace_GankTracker.Program.Main();
                    Chat.Print("Space2Ace - GankTracker Loaded!");
                }

                if (Loader.evade)
                {           XionVade.Program.Game_OnGameStart();
                            Chat.Print("Space2Ace - Evade Loaded!");
                    }

                    if (Loader.cheat)
                    {
                        new TheCheater.TheCheater().Load();
                        Chat.Print("Space2Ace - TheCheater Loaded!");
                    }

                    if (Loader.banwards)
                    {
                        Sebby_Ban_War.Program.Game_OnGameLoad();
                        Chat.Print("Space2Ace - Sebby Banwars Loaded!");

                        /*
                       if (Loader.stream)
                       {
                           StreamBuddy.Program.Main();
                       }

                       if (RandomUltChampsList.Contains(ObjectManager.Player.ChampionName))
                       {
                           if (Loader.randomUlt)
                           {
                               RandomUlt.Program.Game_OnGameLoad();
                           }
                       }

                       if (BaseUltList.Contains(ObjectManager.Player.ChampionName))
                       {
                           if (Loader.baseUlt)
                           {
                               new BaseUlt3.BaseUlt();
                           }
                       }

                   * 
                   */

                        if (Loader.Prediction)
                        {
                            Space2Ace_Prediction.Program.Init();
                            Chat.Print("Space2Ace - Prediction Loaded!");
                        }

                    }
                    if (Loader.traptrack)
                    {
                        AntiTrap.Program.Game_OnGameLoad();
                    }

                    if (!Loader.utilOnly)
                    {
                        switch (ObjectManager.Player.ChampionName.ToLower())
                        {
                            case "aatrox":

                                break;
                            case "akali":

                                break;
                            case "alistar":

                                break;
                            case "amumu":

                                break;
                            case "caitlyn":
                                switch (Loader.cait)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:

                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;
                            case "twitch":
                                switch (Loader.twitch)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:

                                        break;
                                    case 2:

                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;
                            case "ashe":
                                switch (Loader.ashe)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:

                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;
                            case "jayce":
                                switch (Loader.jayce)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:
                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;
                            case "xerath":
                                switch (Loader.xerath)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:

                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;
                            case "ezreal":
                                switch (Loader.ezreal)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:

                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;
                            case "ekko":
                                switch (Loader.ekko)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:

                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;
                            case "graves":
                                switch (Loader.graves)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:

                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;
                            case "ahri":
                                switch (Loader.ahri)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:

                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;

                            case "anivia":
                                switch (Loader.anivia)
                                {
                                    case 0:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 1:

                                        break;
                                    default:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                }
                                break;
                            case "thresh": // OKTW - Sebby - All Seeby champs go down here
                            case "annie":
                            case "jinx":
                            case "karthus":
                            case "missfortune":
                            case "malzahar":
                            case "orianna":
                            case "syndra":
                            case "velkoz":
                            case "swain":
                            case "urgot":
                                SebbyLib.Program.GameOnOnGameLoad();

                                break;
                            case "azir":

                                break;
                            case "bard":

                                break;
                            case "blitzcrank":
                                switch (Loader.blitzcrank)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 2:

                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "brand":
                                switch (Loader.brand)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "cassiopeia":

                                break;
                            case "chogath":

                                break;
                            case "corki":
                                switch (Loader.corki)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    case 2:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "darius":
                                switch (Loader.darius)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "nautilus":
                            case "nunu":
                            case "olaf":
                            case "pantheon":
                            case "renekton":
                            case "tryndamere":

                                break;
                            case "ryze":
                                switch (Loader.ryze)
                                {
                                    case 0:
                                        Space2Ace_Ryze.Program.OnLoad();
                                        Chat.Print("Space2Ace - Ryze Loaded!");
                                        break;
                                    case 1:


                                        break;
                                    default:
                                        Space2Ace_Ryze.Program.OnLoad();
                                        Chat.Print("Space2Ace - Ryze Loaded!");


                                        break;
                                }
                                break;
                            case "diana":
                                switch (Loader.diana)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "drmundo":

                                break;
                            case "draven":
                                switch (Loader.draven)
                                {
                                    case 0:
                                        Space2Ace_Draven.Program.Load();
                                        Chat.Print("Space2Ace - Draven Loaded!");
                                        break;
                                    default:
                                        Space2Ace_Draven.Program.Load();
                                        Chat.Print("Space2Ace - Draven Loaded!");

                                        break;
                                }

                                break;
                            case "elise":
                                switch (Loader.elise)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "evelynn":
                                switch (Loader.evelynn)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "fiddlesticks":

                                break;
                            case "fiora":

                                break;
                            case "fizz":

                                break;
                            case "galio":

                                break;
                            case "gangplank":
                                switch (Loader.gangplank)
                                {
                                    case 0:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "garen":

                                break;
                            case "gnar":

                                break;
                            case "gragas":
                                switch (Loader.gragas)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "hecarim":

                                break;
                            case "heimerdinger":

                                break;
                            case "illaoi":

                                break;
                            case "irelia":

                                break;
                            case "janna":
                                switch (Loader.janna)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "jarvaniv":

                                break;
                            case "jax":
                                switch (Loader.jax)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "jhin":
                                switch (Loader.jhin)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "kalista":
                                switch (Loader.kalista)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    case 2:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "karma":

                                break;
                            case "kassadin":

                                break;
                            case "katarina":
                                switch (Loader.katarina)
                                {
                                    case 0:
                                        Space2Ace_Katarina.Program.Game_OnGameLoad();
                                        Chat.Print("Space2Ace - Champion Katarina Loaded!");

                                        break;
                                    case 1:

                                        break;
                                    default:
                                        //        Space2Ace_Katarina.Program.Game_OnGameLoad();
                                        //        Chat.Print("Space2Ace - Katarina Loaded!");

                                        break;
                                }
                                break;
                            case "kayle":
                                switch (Loader.kayle)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "aurelionsol":

                                break;
                            case "kennen":

                                break;
                            case "khazix":

                                break;
                            case "kindred":
                                switch (Loader.kindred)
                                {
                                    case 0:
                                        Space2Ace_Kindred.Program.Game_OnGameLoad();
                                        Chat.Print("Space2Ace - Champion Kindred Loaded!");
                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:
                                        Space2Ace_Kindred.Program.Game_OnGameLoad();
                                        Chat.Print("Space2Ace - Champion Kindred Loaded!");

                                        break;
                                }
                                break;
                            case "kogmaw":
                                switch (Loader.kogmaw)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    case 2:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "leblanc": // PopBlanc
                                switch (Loader.leblanc)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "leesin": // El Lee Sin
                                switch (Loader.leesin)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    case 2:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "leona": // El Easy

                                break;
                            case "lissandra": // SephLissandra

                                break;
                            case "lucian": // Space2Ace Lucian
                                switch (Loader.lucian)
                                {
                                    case 0:
                                        Space2Ace_Lucian.Program.OnLoad();
                                        Chat.Print("Space2Ace - Champion Lucian Loaded!");
                                        break;
                                    case 1:

                                        break;
                                    case 2:


                                        break;
                                    default:
                                        Space2Ace_Lucian.Program.OnLoad();
                                        Chat.Print("Space2Ace - Champion Lucian Loaded!");
                                        break;
                                }
                                break;
                            case "lulu": // LuluLicious

                                break;
                            case "lux": // MoonLux
                                switch (Loader.lux)
                                {
                                    case 0:
                                        Space2Ace_Lux.Program.GameOnOnGameLoad();
                                        Chat.Print("Space2Ace - Champion Lux Loaded!");

                                        break;
                                    case 1:

                                        break;
                                    default:
                                        Space2Ace_Lux.Program.GameOnOnGameLoad();
                                        Chat.Print("Space2Ace - Champion Lux Loaded!");

                                        break;
                                }
                                break;
                            case "malphite": // eleasy

                        case "vayne": // Space2Ace Lucian
                            switch (Loader.vayne)
                            {
                                case 0:
                                    Space2Ace_Vayne.Vayne.Game_OnGameLoad();
                                    Chat.Print("Space2Ace - Champion Vayne Loaded!");
                                    break;
                                case 1:

                                    break;
                                case 2:


                                    break;
                                default:
                                    Space2Ace_Lucian.Program.OnLoad();
                                    Chat.Print("Space2Ace - Champion Lucian Loaded!");
                                    break;
                            }
                            break;




                        case "quinn": // GFuel Quinn & OKTW
                                switch (Loader.quinn)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "tristana": // ElTristana

                                break;
                            case "taliyah": // taliyah && tophsharp
                                switch (Loader.taliyah)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "riven": // Nechrito Riven & Badao Riven
                                switch (Loader.riven)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    case 2:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "talon": // GFuel Talon

                                break;
                            case "zed":
                                switch (Loader.zed)
                                {
                                    case 0:
                                        Space2Ace_Zed.Program.Game_OnGameLoad();
                                        Chat.Print("Space2Ace - Champion Zed Loaded!");
                                        Notifications.Show(new SimpleNotification("Zed", "Enable Space2Ace Prediction in Misc Menue for ZED !!!!"), 30000);
                                        break;
                                    case 1:

                                        break;
                                    case 2:

                                        break;
                                    default:
                                        Space2Ace_Zed.Program.Game_OnGameLoad();
                                        Chat.Print("Space2Ace - Champion Zed Loaded!");
                                        Notifications.Show(new SimpleNotification("Zed", "Enable Space2Ace Prediction in Misc Menue for ZED !!!!"), 30000);


                                        break;
                                }
                                break;
                            case "udyr": // D_Udyr

                                break;
                            case "maokai": // Underrated AIO

                                break;
                            case "masteryi": // MasterSharp

                                break;
                            case "mordekaiser": // How to Train your dragon

                                break;
                            case "morgana": // Kurisu Morg & OKTW
                                switch (Loader.morgana)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "nami": // vSupport Series

                                break;
                            case "nasus": // Underrated AIO

                                break;
                            case "nidalee":
                                switch (Loader.nidalee)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "yasuo": // YasuPro
                                switch (Loader.yasuo)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        break;
                                    case 2:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "nocturne": // Underrated AIO

                                break;
                            case "poppy": // Underrated AIO
                                switch (Loader.poppy)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "rammus": // BrianSharp

                                break;
                            case "rengar": // ElRengar && D-Rengar
                                switch (Loader.rengar)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "soraka": // Sophie's Soraka
                                switch (Loader.soraka)
                                {

                                    case 0:
                                        Space2Ace_Soraka.Program.Main();
                                        Chat.Print("Space2Ace - Champion Soraka Loaded!");

                                        break;

                                    default:
                                        Space2Ace_Soraka.Program.Main();
                                        Chat.Print("Space2Ace - Champion Soraka Loaded!");
                                        break;
                                }
                                break;
                            case "twistedfate": // Twisted Fate by Kortatu & OKTW
                                switch (Loader.twistedfate)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        SebbyLib.Program.GameOnOnGameLoad();
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "varus": // ElVarus

                                break;
                            case "veigar": // FreshBooster

                                break;
                            case "reksai": // D-Reksai && HeavenStrikeReksaj
                                switch (Loader.reksai)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "rumble": // Underrated AIO & ElRumble
                                switch (Loader.rumble)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "sejuani": // ElSejuani

                                break;
                            case "shaco": // Underrated AIO & ChewyMoon's Shaco
                                switch (Loader.shaco)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "shen": // Underrated AIO

                                break;
                            case "skarner": // Underrated AIO

                                break;
                            case "sona": // vSeries Support & ElEasy Sona
                                switch (Loader.sona)
                                {
                                    case 0:

                                        break;
                                    case 1:
                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "teemo": // Sharpshooter

                                break;
                            case "viktor": // Trus In my Viktor

                                break;
                            case "vladimir": // ElVlad

                                break;
                            case "warwick": // Warwick - Mirin

                                break;
                            case "monkeyking": // Wukong - xQx

                                break;
                            case "xinzhao": // Xin xQx

                                break;
                            case "ziggs": // Ziggs#

                                break;
                            case "yorick": // UnderratedAIO

                                break;
                            case "zyra": // D-Zyra

                                break;
                            case "zilean": // ElZilean

                                break;
                            case "shyvana": // D-Shyvana

                                break;
                            case "singed": // ElSinged

                                break;
                            case "zac": // Underrated AIO

                                break;
                            case "tahmkench": // Underrated AIO

                                break;
                            case "sion": // Underrated AIO
                                switch (Loader.sion)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "vi": //ElVi

                                break;
                            case "volibear": // Underrated AIO && VoliPower
                                switch (Loader.volibear)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "trundle": // ElTrundle
                                switch (Loader.trundle)
                                {
                                    case 0:

                                        break;
                                    case 1:

                                        break;
                                    default:

                                        break;
                                }
                                break;
                            case "taric": // SkyLv_Taric

                                break;
                            default:
                                Chat.Print("This champion is not supported yet but the utilities will still load! - Xion");
                                break;
                        }
                    }
                }
            }
        }
    }
