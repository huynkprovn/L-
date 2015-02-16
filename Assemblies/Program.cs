// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Assemblies.Champions;
using LeagueSharp;
using LeagueSharp.Common;

namespace Assemblies
{
    internal static class Program
    {
        private static Champion _champion;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                switch (ObjectManager.Player.ChampionName)
                {
                    case "Ezreal":
                        _champion = new Ezreal();
                        break;
                    case "Fizz":
                        _champion = new Fizz();
                        break;
                    case "Kalista":
                        _champion = new Kalista();
                        break;
                    case "Irelia":
                        _champion = new Irelia();
                        break;
                    case "Gnar":
                        _champion = new Gnar();
                        break;
                    default:
                        _champion = new Champion();
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Fail.");
            }
        }
    }
}