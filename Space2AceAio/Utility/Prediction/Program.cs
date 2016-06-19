using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Space2AcePrediction;
using EloBuddy;

namespace Space2Ace_Prediction
{
    class Program
    {
        public static void Init()
        {
            CustomEvents.Game.OnGameLoad += EventHandlers.Game_OnGameLoad;
        }
    }
}