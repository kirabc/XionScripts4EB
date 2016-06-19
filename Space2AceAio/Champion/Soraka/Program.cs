
using Space2Ace_Soraka;
using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

namespace Space2Ace_Soraka
{
    class Program
    {
        public static void Main()
        {
          
            Events.OnLoad += (sender, eventArgs) =>
            {
                switch (ObjectManager.Player.ChampionName)
                {
                    case "Soraka":
                        new Soraka();
                        break;

                  
                    default:
                        //Variables.Orbwalker.Enabled = false;
                        break;
                }
            };
        }
    }
}